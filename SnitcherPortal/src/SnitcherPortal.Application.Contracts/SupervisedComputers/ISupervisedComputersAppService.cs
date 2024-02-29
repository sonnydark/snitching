using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SnitcherPortal.SupervisedComputers
{
    public interface ISupervisedComputersAppService : IApplicationService
    {
        Task KillProcessAsync(string computerName, string processName);
        Task SendMessageAsync(string computerName, string message);
        Task<List<string>> GetAvailableComputersAsync();
        Task<DashboardDataDto> GetDashboardDataAsync(string computerName);


        Task<PagedResultDto<SupervisedComputerDto>> GetListAsync(GetSupervisedComputersInput input);

        Task<SupervisedComputerDto> GetAsync(Guid id);
        Task<SupervisedComputerDto> GetAsync(string connectionId);

        Task DeleteAsync(Guid id);

        Task<SupervisedComputerDto> CreateAsync(SupervisedComputerCreateDto input);

        Task<SupervisedComputerDto> UpdateAsync(Guid id, SupervisedComputerUpdateDto input);
    }
}