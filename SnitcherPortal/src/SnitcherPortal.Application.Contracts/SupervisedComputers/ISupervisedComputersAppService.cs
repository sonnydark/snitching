using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SnitcherPortal.SupervisedComputers
{
    public interface ISupervisedComputersAppService : IApplicationService
    {

        Task<PagedResultDto<SupervisedComputerDto>> GetListAsync(GetSupervisedComputersInput input);

        Task<SupervisedComputerDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<SupervisedComputerDto> CreateAsync(SupervisedComputerCreateDto input);

        Task<SupervisedComputerDto> UpdateAsync(Guid id, SupervisedComputerUpdateDto input);
    }
}