using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SnitcherPortal.SnitchingLogs
{
    public interface ISnitchingLogsAppService : IApplicationService
    {
        Task<PagedResultDto<SnitchingLogDto>> GetListBySupervisedComputerIdAsync(GetSnitchingLogListInput input);

        Task<PagedResultDto<SnitchingLogDto>> GetListAsync(GetSnitchingLogsInput input);

        Task<SnitchingLogDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<SnitchingLogDto> CreateAsync(SnitchingLogCreateDto input);

        Task<SnitchingLogDto> UpdateAsync(Guid id, SnitchingLogUpdateDto input);
    }
}