using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SnitcherPortal.ActivityRecords
{
    public interface IActivityRecordsAppService : IApplicationService
    {
        Task<PagedResultDto<ActivityRecordDto>> GetListBySupervisedComputerIdAsync(GetActivityRecordListInput input);

        Task<PagedResultDto<ActivityRecordDto>> GetListAsync(GetActivityRecordsInput input);

        Task<ActivityRecordDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ActivityRecordDto> CreateAsync(ActivityRecordCreateDto input);

        Task<ActivityRecordDto> UpdateAsync(Guid id, ActivityRecordUpdateDto input);
    }
}