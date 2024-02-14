using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SnitcherPortal.KnownProcesses
{
    public interface IKnownProcessesAppService : IApplicationService
    {
        Task<PagedResultDto<KnownProcessDto>> GetListBySupervisedComputerIdAsync(GetKnownProcessListInput input);

        Task<PagedResultDto<KnownProcessDto>> GetListAsync(GetKnownProcessesInput input);

        Task<KnownProcessDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<KnownProcessDto> CreateAsync(KnownProcessCreateDto input);

        Task<KnownProcessDto> UpdateAsync(Guid id, KnownProcessUpdateDto input);
        Task MarkUnmarkHiddenAsync(Guid scId, bool mark);
    }
}