using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace SnitcherPortal.ActivityRecords
{
    public class ActivityRecordManager : DomainService
    {
        protected IActivityRecordRepository _activityRecordRepository;

        public ActivityRecordManager(IActivityRecordRepository activityRecordRepository)
        {
            _activityRecordRepository = activityRecordRepository;
        }

        public virtual async Task<ActivityRecord> CreateAsync(
        Guid supervisedComputerId, DateTime startTime, DateTime? endTime = null, string? data = null)
        {
            Check.NotNull(startTime, nameof(startTime));

            var activityRecord = new ActivityRecord(
             GuidGenerator.Create(),
             supervisedComputerId, startTime, endTime, data
             );

            return await _activityRecordRepository.InsertAsync(activityRecord);
        }

        public virtual async Task<ActivityRecord> UpdateAsync(
            Guid id,
            Guid supervisedComputerId, DateTime startTime, DateTime? endTime = null, string? data = null
        )
        {
            Check.NotNull(startTime, nameof(startTime));

            var activityRecord = await _activityRecordRepository.GetAsync(id);

            activityRecord.SupervisedComputerId = supervisedComputerId;
            activityRecord.StartTime = startTime;
            activityRecord.EndTime = endTime;
            activityRecord.Data = data;

            return await _activityRecordRepository.UpdateAsync(activityRecord);
        }

    }
}