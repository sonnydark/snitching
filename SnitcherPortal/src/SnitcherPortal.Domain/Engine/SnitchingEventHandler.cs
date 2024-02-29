using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Tls;
using Polly;
using SnitcherPortal.ActivityRecords;
using SnitcherPortal.Calendars;
using SnitcherPortal.KnownProcesses;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.SupervisedComputers;
using System;
using System.Linq;
using System.Text.Json;
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

                    supervisedComputer ??= await _supervisedComputerManager
                            .CreateAsync("Autoregistered", eventData.MachineIdentifier!, true, 10, false, "", null);

                    // Handle computer properties
                    supervisedComputer.Status = SupervisedComputerStatus.ONLINE;
                    if (supervisedComputer.ConnectionId != eventData.ConnectionId)
                    {
                        await _localEventBus.PublishAsync(new SetConfigurationEto()
                        {
                            ConnectionId = eventData.ConnectionId,
                            Heartbeat = supervisedComputer.ClientHeartbeat * 1000
                        }, false);
                    }

                    supervisedComputer.ConnectionId = eventData.ConnectionId;

                    // Handle snitching log
                    if (eventData.Logs?.Count > 0)
                    {
                        var snitchingLogs = eventData.Logs.Select(e => new SnitchingLog(Guid.NewGuid(), supervisedComputer.Id, DateTime.Now, e)).ToList();
                        await _snitchingLogRepository.InsertManyAsync(snitchingLogs);
                    }

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
                        .Where(e => e.SupervisedComputerId == supervisedComputer.Id)
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
                        activityRecord = new ActivityRecord(Guid.NewGuid(), supervisedComputer.Id, now, null, null)
                        {
                            LastUpdateTime = now
                        };
                        activityRecords.Add(activityRecord);
                        await _activityRecordRepository.InsertAsync(activityRecord);
                    }

                    // Handle calendar impliances
                    await HandleProcessKillingAsync(eventData, supervisedComputer, activityRecord);

                    // Map refresh UI event and commit changes
                    dashboardDataDto = DashboardMapper
                        .Map(supervisedComputer, activityRecords.OrderByDescending(e => e.StartTime).Take(10).ToList(), eventData.Processes);

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

        public async Task HandleProcessKillingAsync(SnitchingDataEto eventData, SupervisedComputer supervisedComputer, ActivityRecord activityRecord)
        {
            var banList = supervisedComputer.KnownProcesses.Where(e => e.IsImportant).Select(e => e.Name).ToList();
            var targetList = eventData!.Processes!.Where(p => banList.Any(b => p.Contains(b))).ToList();

            if (targetList.Count == 0)
            {
                return;
            }

            var now = DateTime.Now;
            if (supervisedComputer.BanUntil.HasValue && supervisedComputer.BanUntil >= now)
            {
                activityRecord.Data += activityRecord.Data.IsNullOrWhiteSpace() == false ? "; " : "";
                activityRecord.Data += $"'{string.Join(", ", targetList)}' killed based on BanUntil at {DateTime.Now:HH:mm:ss}";

                await _localEventBus.PublishAsync(new KillCommandEto() { ConnectionId = supervisedComputer.ConnectionId, Processes = targetList }, false);

                if (supervisedComputer.EnableAutokillReasoning)
                {
                    await _localEventBus.PublishAsync(new ShowMessageEto()
                    {
                        ConnectionId = supervisedComputer.ConnectionId,
                        Duration = 4,
                        Message = "Ukoncene kvoli nastaveniu obmedzenia BanUntil"
                    }, false);
                }
                return;
            }

            var todaysCalendar = supervisedComputer.Calendars.FirstOrDefault(e => e.DayOfWeek == (int)now.DayOfWeek);
            if (todaysCalendar == null || todaysCalendar.AllowedHours.IsNullOrEmpty())
            {
                return;
            }

            CalendarSettingsJson? calendarSettings;
            try
            {
                calendarSettings = JsonSerializer.Deserialize<CalendarSettingsJson>(todaysCalendar.AllowedHours);
                if (calendarSettings == null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return;
            }

            var nowTimespan = new TimeSpan(now.Hour, now.Minute, now.Second);
            if (!calendarSettings.Hours.Any(e => e.Start <= nowTimespan && e.End >= nowTimespan))
            {
                activityRecord.Data += activityRecord.Data.IsNullOrWhiteSpace() == false ? "; " : "";
                activityRecord.Data += $"'{string.Join(", ", targetList)}' killed based on AllowedHours at {DateTime.Now:HH:mm:ss}";

                await _localEventBus.PublishAsync(new KillCommandEto() { ConnectionId = supervisedComputer.ConnectionId, Processes = targetList }, false);

                if (supervisedComputer.EnableAutokillReasoning)
                {
                    await _localEventBus.PublishAsync(new ShowMessageEto()
                    {
                        ConnectionId = supervisedComputer.ConnectionId,
                        Duration = 4,
                        Message = "Ukoncene kvoli nastaveniu obmedzenia AllowedHours"
                    }, false);
                }
                return;
            }

            var records = (await _activityRecordRepository.GetQueryableNoTrackingAsync(false))
                .Where(e => e.SupervisedComputerId == supervisedComputer.Id && (e.EndTime == null
                || (e.EndTime > now.Date))).ToList();
            double seconds = 0;
            foreach (var record in records)
            {
                if (record.StartTime < now.Date)
                {
                    if (record.EndTime.HasValue)
                    {
                        seconds += (record.EndTime! - now.Date).Value.TotalSeconds;
                    }
                    else
                    {
                        seconds += (now - now.Date).TotalSeconds;
                    }
                }
                else
                {
                    if (record.EndTime.HasValue)
                    {
                        seconds += (record.EndTime! - record.StartTime).Value.TotalSeconds;
                    }
                    else
                    {
                        seconds += (now - record.StartTime).TotalSeconds;
                    }
                }
            }

            if (calendarSettings.Quota * 60 * 60 < seconds)
            {
                activityRecord.Data += activityRecord.Data.IsNullOrWhiteSpace() == false ? "; " : "";
                activityRecord.Data += $"'{string.Join(", ", targetList)}' killed based on AllowedHours at {DateTime.Now:HH:mm:ss}";

                await _localEventBus.PublishAsync(new KillCommandEto() { ConnectionId = supervisedComputer.ConnectionId, Processes = targetList }, false);

                if (supervisedComputer.EnableAutokillReasoning)
                {
                    await _localEventBus.PublishAsync(new ShowMessageEto()
                    {
                        ConnectionId = supervisedComputer.ConnectionId,
                        Duration = 4,
                        Message = "Ukoncene kvoli nastaveniu obmedzenia Quota"
                    }, false);
                }
                return;
            }
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
