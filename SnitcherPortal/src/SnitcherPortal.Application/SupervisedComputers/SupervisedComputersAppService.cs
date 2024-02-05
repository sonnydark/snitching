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
using SnitcherPortal.SupervisedComputers;

namespace SnitcherPortal.SupervisedComputers
{

    [Authorize(SnitcherPortalPermissions.SupervisedComputers.Default)]
    public class SupervisedComputersAppService : ApplicationService, ISupervisedComputersAppService
    {

        protected ISupervisedComputerRepository _supervisedComputerRepository;
        protected SupervisedComputerManager _supervisedComputerManager;

        public SupervisedComputersAppService(ISupervisedComputerRepository supervisedComputerRepository, SupervisedComputerManager supervisedComputerManager)
        {

            _supervisedComputerRepository = supervisedComputerRepository;
            _supervisedComputerManager = supervisedComputerManager;
        }

        public virtual async Task<PagedResultDto<SupervisedComputerDto>> GetListAsync(GetSupervisedComputersInput input)
        {
            var totalCount = await _supervisedComputerRepository.GetCountAsync(input.FilterText, input.Name, input.Identifier, input.IpAddress, input.IsCalendarActive, input.BanUntilMin, input.BanUntilMax);
            var items = await _supervisedComputerRepository.GetListAsync(input.FilterText, input.Name, input.Identifier, input.IpAddress, input.IsCalendarActive, input.BanUntilMin, input.BanUntilMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SupervisedComputerDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SupervisedComputer>, List<SupervisedComputerDto>>(items)
            };
        }

        public virtual async Task<SupervisedComputerDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(await _supervisedComputerRepository.GetAsync(id));
        }

        [Authorize(SnitcherPortalPermissions.SupervisedComputers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _supervisedComputerRepository.DeleteAsync(id);
        }

        [Authorize(SnitcherPortalPermissions.SupervisedComputers.Create)]
        public virtual async Task<SupervisedComputerDto> CreateAsync(SupervisedComputerCreateDto input)
        {

            var supervisedComputer = await _supervisedComputerManager.CreateAsync(
            input.Name, input.Identifier, input.IsCalendarActive, input.IpAddress, input.BanUntil
            );

            return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(supervisedComputer);
        }

        [Authorize(SnitcherPortalPermissions.SupervisedComputers.Edit)]
        public virtual async Task<SupervisedComputerDto> UpdateAsync(Guid id, SupervisedComputerUpdateDto input)
        {

            var supervisedComputer = await _supervisedComputerManager.UpdateAsync(
            id,
            input.Name, input.Identifier, input.IsCalendarActive, input.IpAddress, input.BanUntil, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(supervisedComputer);
        }
    }
}