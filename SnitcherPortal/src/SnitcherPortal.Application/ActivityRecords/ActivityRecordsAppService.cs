using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using SnitcherPortal.Permissions;
using SnitcherPortal.ActivityRecords;

namespace SnitcherPortal.ActivityRecords
{

    [Authorize(SnitcherPortalPermissions.ActivityRecords.Default)]
    public class ActivityRecordsAppService : ApplicationService, IActivityRecordsAppService
    {

        protected IActivityRecordRepository _activityRecordRepository;
        protected ActivityRecordManager _activityRecordManager;

        public ActivityRecordsAppService(IActivityRecordRepository activityRecordRepository, ActivityRecordManager activityRecordManager)
        {

            _activityRecordRepository = activityRecordRepository;
            _activityRecordManager = activityRecordManager;
        }

        public virtual async Task<PagedResultDto<ActivityRecordDto>> GetListBySupervisedComputerIdAsync(GetActivityRecordListInput input)
        {
            var activityRecords = await _activityRecordRepository.GetListBySupervisedComputerIdAsync(
                input.SupervisedComputerId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<ActivityRecordDto>
            {
                TotalCount = await _activityRecordRepository.GetCountBySupervisedComputerIdAsync(input.SupervisedComputerId),
                Items = ObjectMapper.Map<List<ActivityRecord>, List<ActivityRecordDto>>(activityRecords)
            };
        }

        public virtual async Task<PagedResultDto<ActivityRecordDto>> GetListAsync(GetActivityRecordsInput input)
        {
            var totalCount = await _activityRecordRepository.GetCountAsync(input.FilterText, input.StartTimeMin, input.StartTimeMax, input.EndTimeMin, input.EndTimeMax, input.Data);
            var items = await _activityRecordRepository.GetListAsync(input.FilterText, input.StartTimeMin, input.StartTimeMax, input.EndTimeMin, input.EndTimeMax, input.Data, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ActivityRecordDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ActivityRecord>, List<ActivityRecordDto>>(items)
            };
        }

        public virtual async Task<ActivityRecordDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ActivityRecord, ActivityRecordDto>(await _activityRecordRepository.GetAsync(id));
        }

        [Authorize(SnitcherPortalPermissions.ActivityRecords.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _activityRecordRepository.DeleteAsync(id);
        }

        [Authorize(SnitcherPortalPermissions.ActivityRecords.Create)]
        public virtual async Task<ActivityRecordDto> CreateAsync(ActivityRecordCreateDto input)
        {

            var activityRecord = await _activityRecordManager.CreateAsync(input.SupervisedComputerId,
            input.StartTime, input.EndTime, input.Data
            );

            return ObjectMapper.Map<ActivityRecord, ActivityRecordDto>(activityRecord);
        }

        [Authorize(SnitcherPortalPermissions.ActivityRecords.Edit)]
        public virtual async Task<ActivityRecordDto> UpdateAsync(Guid id, ActivityRecordUpdateDto input)
        {

            var activityRecord = await _activityRecordManager.UpdateAsync(
            id, input.SupervisedComputerId,
            input.StartTime, input.EndTime, input.Data
            );

            return ObjectMapper.Map<ActivityRecord, ActivityRecordDto>(activityRecord);
        }
    }
}