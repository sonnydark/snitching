using Microsoft.Extensions.Logging;
using SnitcherPortal.KnownProcesses;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.SupervisedComputers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace SnitcherPortal.Engine
{
    public class SnitchingJob : ISingletonDependency
    {
        private static int RefreshPeriod = 1000;

        protected KnownProcessManager _knownProcessManager;
        protected IUnitOfWorkManager _unitOfWorkManager;
        protected SnitcherClientFunctions _snitcherClientFunctions;
        protected ISupervisedComputerRepository _supervisedComputerRepository;
        protected ISnitchingLogRepository _snitchingLogRepository;
        protected ILogger<SnitchingJob> _logger;

        public SnitchingJob(IUnitOfWorkManager unitOfWorkManager,
            KnownProcessManager knownProcessManager,
            SnitcherClientFunctions snitcherClientFunctions,
            ISupervisedComputerRepository supervisedComputerRepository,
            ISnitchingLogRepository snitchingLogRepository,
            ILogger<SnitchingJob> logger)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _knownProcessManager = knownProcessManager;
            _snitcherClientFunctions = snitcherClientFunctions;
            _supervisedComputerRepository = supervisedComputerRepository;
            _snitchingLogRepository = snitchingLogRepository;
            _logger = logger;
        }

        public void Start()
        {
            Task.Run(SnitchingLoop);
        }

        private async Task SnitchingLoop()
        {
            try
            {
                List<SupervisedComputer> scList = [];
                using (var unitOfWork = _unitOfWorkManager.Begin(true))
                {
                    scList = (await _supervisedComputerRepository.GetQueryableNoTrackingAsync()).ToList();
                }

                foreach (var scItem in scList.Where(e => e.IpAddress?.Length > 0))
                {
                    await HandleSupervisedComputer(scItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                Thread.Sleep(RefreshPeriod);
                await Task.Run(SnitchingLoop);
            }
        }

        private async Task HandleSupervisedComputer(SupervisedComputer input)
        {
            SnitchingDataDto? data = null;
            try
            {
                data = await _snitcherClientFunctions.GetShitchingData(input.IpAddress!);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin(true))
                {
                    var supervisedComputer = (await _supervisedComputerRepository.WithDetailsAsync()).Where(sc => sc.Id == input.Id).First();

                    // Handle offline state
                    if (data == null)
                    {
                        supervisedComputer.Status = SupervisedComputerStatus.OFFLINE;
                        await unitOfWork.CompleteAsync();
                        return;
                    }

                    // Handle computer properties
                    supervisedComputer.Status = SupervisedComputerStatus.ONLINE;
                    supervisedComputer.Identifier = data.MachineIdentifier ?? supervisedComputer.Identifier;

                    // Handle snitching log
                    if (data.Logs?.Count > 0)
                    {
                        data.Logs.ForEach(e =>
                        {
                            supervisedComputer.SnitchingLogs.Add(new SnitchingLog(Guid.NewGuid(), supervisedComputer.Id, DateTime.Now, e));
                        });
                    }
                    var logsToRemove = supervisedComputer.SnitchingLogs.Where(e => e.Timestamp > DateTime.Now.AddDays(-7)).ToList();
                    await _snitchingLogRepository.DeleteManyAsync(logsToRemove);

                    // Handle processes
                    if (data.Processes?.Count > 0)
                    {
                        var newlyDetectedProcesses = data.Processes.Except(supervisedComputer.KnownProcesses.Select(kp => kp.Name)).ToList();
                        newlyDetectedProcesses.ForEach(p =>
                        {
                            supervisedComputer.KnownProcesses.Add(new KnownProcess(Guid.NewGuid(), supervisedComputer.Id, p, false, false));
                        });
                    }

                    await _supervisedComputerRepository.UpdateAsync(supervisedComputer);
                    await unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
    }
}
