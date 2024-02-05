using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace SnitcherPortal.SnitchingLogs
{
    public class SnitchingLogManager : DomainService
    {
        protected ISnitchingLogRepository _snitchingLogRepository;

        public SnitchingLogManager(ISnitchingLogRepository snitchingLogRepository)
        {
            _snitchingLogRepository = snitchingLogRepository;
        }

        public virtual async Task<SnitchingLog> CreateAsync(
        Guid supervisedComputerId, DateTime timestamp, string? message = null)
        {
            Check.NotNull(timestamp, nameof(timestamp));

            var snitchingLog = new SnitchingLog(
             GuidGenerator.Create(),
             supervisedComputerId, timestamp, message
             );

            return await _snitchingLogRepository.InsertAsync(snitchingLog);
        }

        public virtual async Task<SnitchingLog> UpdateAsync(
            Guid id,
            Guid supervisedComputerId, DateTime timestamp, string? message = null
        )
        {
            Check.NotNull(timestamp, nameof(timestamp));

            var snitchingLog = await _snitchingLogRepository.GetAsync(id);

            snitchingLog.SupervisedComputerId = supervisedComputerId;
            snitchingLog.Timestamp = timestamp;
            snitchingLog.Message = message;

            return await _snitchingLogRepository.UpdateAsync(snitchingLog);
        }

    }
}