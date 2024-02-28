using Microsoft.Extensions.Logging;
using SnitcherPortal.ActivityRecords;
using SnitcherPortal.KnownProcesses;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.SupervisedComputers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;
using static EtoDefinitions;

namespace SnitcherPortal.Engine
{
    public class SnitchingEventHandler : ILocalEventHandler<SnitchingDataEto>, ILocalEventHandler<ClientDisconnectEto>, ITransientDependency
    {
        protected KnownProcessManager _knownProcessManager;
        protected IUnitOfWorkManager _unitOfWorkManager;
        protected SnitcherClientFunctions _snitcherClientFunctions;
        protected ISupervisedComputerRepository _supervisedComputerRepository;
        protected SupervisedComputerManager _supervisedComputerManager;
        protected IActivityRecordRepository _activityRecordRepository;
        protected ISnitchingLogRepository _snitchingLogRepository;
        protected ILogger<SnitchingEventHandler> _logger;
        protected ILocalEventBus _localEventBus;

        public SnitchingEventHandler(IUnitOfWorkManager unitOfWorkManager,
            KnownProcessManager knownProcessManager,
            SnitcherClientFunctions snitcherClientFunctions,
            ISupervisedComputerRepository supervisedComputerRepository,
            SupervisedComputerManager supervisedComputerManager,
            IActivityRecordRepository activityRecordRepository,
            ISnitchingLogRepository snitchingLogRepository,
            ILogger<SnitchingEventHandler> logger,
            ILocalEventBus localEventBus)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _knownProcessManager = knownProcessManager;
            _snitcherClientFunctions = snitcherClientFunctions;
            _supervisedComputerRepository = supervisedComputerRepository;
            _supervisedComputerManager = supervisedComputerManager;
            _activityRecordRepository = activityRecordRepository;
            _snitchingLogRepository = snitchingLogRepository;
            _logger = logger;
            _localEventBus = localEventBus;
        }

        public async Task HandleEventAsync(ClientDisconnectEto eventData)
        {
            try
            {
                DashboardDataDto dashboardDataDto;
                using (var unitOfWork = _unitOfWorkManager.Begin(true))
                {
                    var supervisedComputer = (await _supervisedComputerRepository.WithDetailsAsync())
                        .FirstOrDefault(sc => sc.ConnectionId == eventData!.ConnectionId);

                    if (supervisedComputer == null)
                    {
                        return;
                    }

                    var activityRecords = (await _activityRecordRepository.GetQueryableAsync())
                        .OrderByDescending(e => e.StartTime).Take(10).ToList();
                    supervisedComputer.Status = SupervisedComputerStatus.OFFLINE;
                    await _supervisedComputerRepository.UpdateAsync(supervisedComputer);
                    await unitOfWork.CompleteAsync();

                    dashboardDataDto = DashboardMapper
                        .Map(supervisedComputer, activityRecords.OrderByDescending(e => e.StartTime).Take(10).ToList(), null);
                }
                
                await _localEventBus.PublishAsync(dashboardDataDto, false);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        public async Task HandleEventAsync(SnitchingDataEto eventData)
        {
            if (eventData == null || (eventData.MachineIdentifier?.IsNullOrWhiteSpace() ?? true) || (eventData.ConnectionId?.IsNullOrWhiteSpace() ?? true))
            {
                _logger.LogError("Received data from Client is not valid, will be ignored!");
                return;
            }

            try
            {
                DashboardDataDto dashboardDataDto;

                using (var unitOfWork = _unitOfWorkManager.Begin(true))
                {
                    var supervisedComputer = (await _supervisedComputerRepository.WithDetailsAsync())
                        .FirstOrDefault(sc => sc.Identifier == eventData.MachineIdentifier);

                    if (supervisedComputer == null)
                    {
                        supervisedComputer = await _supervisedComputerManager
                            .CreateAsync("Autoregistered", eventData.MachineIdentifier!, true, "", null);
                    }

                    // Handle computer properties
                    supervisedComputer.Status = SupervisedComputerStatus.ONLINE;
                    supervisedComputer.ConnectionId = eventData.ConnectionId;

                    // Handle snitching log
                    if (eventData.Logs?.Count > 0)
                    {
                        eventData.Logs.ForEach(e =>
                        {
                            supervisedComputer.SnitchingLogs.Add(new SnitchingLog(Guid.NewGuid(), supervisedComputer.Id, DateTime.Now, e));
                        });
                    }
                    var logsToRemove = supervisedComputer.SnitchingLogs.Where(e => e.Timestamp < DateTime.Now.AddDays(-7)).ToList();
                    await _snitchingLogRepository.DeleteManyAsync(logsToRemove);

                    // Handle processes
                    if (eventData.Processes?.Count > 0)
                    {
                        var newlyDetectedProcesses = eventData.Processes.Except(supervisedComputer.KnownProcesses.Select(kp => kp.Name)).ToList();
                        newlyDetectedProcesses.ForEach(p =>
                        {
                            supervisedComputer.KnownProcesses.Add(new KnownProcess(Guid.NewGuid(), supervisedComputer.Id, p, false, false));
                        });
                    }

                    // Handle activity records
                    var now = DateTime.Now;
                    var activityRecords = (await _activityRecordRepository.GetQueryableAsync())
                        .OrderByDescending(e => e.StartTime).Take(10).ToList();

                    var activityRecord = activityRecords.FirstOrDefault(ar => ar.EndTime == null);
                    if (activityRecord != null)
                    {
                        if (activityRecord.LastUpdateTime < now.AddMinutes(-10))
                        {
                            activityRecord.EndTime = activityRecord.LastUpdateTime;
                            await _activityRecordRepository.UpdateAsync(activityRecord);
                            activityRecord = null;
                        }
                        else
                        {
                            activityRecord.LastUpdateTime = now;
                            await _activityRecordRepository.UpdateAsync(activityRecord);
                        }
                    }

                    if (activityRecord == null)
                    {
                        activityRecord = new ActivityRecord(Guid.NewGuid(), supervisedComputer.Id, now, null, null);
                        activityRecord.LastUpdateTime = now;
                        activityRecords.Add(activityRecord);
                        await _activityRecordRepository.InsertAsync(activityRecord);
                    }
                    dashboardDataDto = DashboardMapper
                        .Map(supervisedComputer, activityRecords.OrderByDescending(e => e.StartTime).Take(10).ToList(), eventData.Processes);

                    // Commit changes
                    await _supervisedComputerRepository.UpdateAsync(supervisedComputer);
                    await unitOfWork.CompleteAsync();
                }
                
                await _localEventBus.PublishAsync(dashboardDataDto, false);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        public async Task HandleProcessKillingAsync()
        {
            //sem rovno param kvoli load times, ale save uz nie
            // spravit toto po zobrazeni, mozno s delay XY s vlastnym await _localEventBus.PublishAsync(dashboardDataDto) .. potom to bude vyzerat ze preblikava?

            //await _localEventBus.PublishAsync(new ShowMessageEto()
            //{
            //    ConnectionId = supervisedComputer.ConnectionId,
            //    Duration = 1,
            //    Message = "Toto je naozaj dlha sprava.Toto je naozaj dlha sprava.Toto je naozaj dlha sprava.Toto je naozaj dlha sprava.Toto je naozaj dlha sprava.Toto je naozaj dlha sprava.Toto je naozaj dlha sprava.Toto je naozaj dlha sprava."
            //}, false);
        }

        public async Task TriggerDashboardChangedAsync(DashboardDataDto data)
        {
            try
            {
                await _localEventBus.PublishAsync(data);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
        
    }
}
