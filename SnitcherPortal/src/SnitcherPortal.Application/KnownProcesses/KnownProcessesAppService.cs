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
using SnitcherPortal.KnownProcesses;

namespace SnitcherPortal.KnownProcesses
{

    [Authorize(SnitcherPortalPermissions.KnownProcesses.Default)]
    public class KnownProcessesAppService : ApplicationService, IKnownProcessesAppService
    {

        protected IKnownProcessRepository _knownProcessRepository;
        protected KnownProcessManager _knownProcessManager;

        public KnownProcessesAppService(IKnownProcessRepository knownProcessRepository, KnownProcessManager knownProcessManager)
        {

            _knownProcessRepository = knownProcessRepository;
            _knownProcessManager = knownProcessManager;
        }

        public virtual async Task<PagedResultDto<KnownProcessDto>> GetListBySupervisedComputerIdAsync(GetKnownProcessListInput input)
        {
            var knownProcesses = await _knownProcessRepository.GetListBySupervisedComputerIdAsync(
                input.SupervisedComputerId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<KnownProcessDto>
            {
                TotalCount = await _knownProcessRepository.GetCountBySupervisedComputerIdAsync(input.SupervisedComputerId),
                Items = ObjectMapper.Map<List<KnownProcess>, List<KnownProcessDto>>(knownProcesses)
            };
        }

        public virtual async Task<PagedResultDto<KnownProcessDto>> GetListAsync(GetKnownProcessesInput input)
        {
            var totalCount = await _knownProcessRepository.GetCountAsync(input.FilterText, input.Name, input.IsHidden, input.IsImportant);
            var items = await _knownProcessRepository.GetListAsync(input.FilterText, input.Name, input.IsHidden, input.IsImportant, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<KnownProcessDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<KnownProcess>, List<KnownProcessDto>>(items)
            };
        }

        public virtual async Task<KnownProcessDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<KnownProcess, KnownProcessDto>(await _knownProcessRepository.GetAsync(id));
        }

        [Authorize(SnitcherPortalPermissions.KnownProcesses.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _knownProcessRepository.DeleteAsync(id);
        }

        [Authorize(SnitcherPortalPermissions.KnownProcesses.Create)]
        public virtual async Task<KnownProcessDto> CreateAsync(KnownProcessCreateDto input)
        {

            var knownProcess = await _knownProcessManager.CreateAsync(input.SupervisedComputerId,
            input.Name, input.IsHidden, input.IsImportant
            );

            return ObjectMapper.Map<KnownProcess, KnownProcessDto>(knownProcess);
        }

        [Authorize(SnitcherPortalPermissions.KnownProcesses.Edit)]
        public virtual async Task<KnownProcessDto> UpdateAsync(Guid id, KnownProcessUpdateDto input)
        {

            var knownProcess = await _knownProcessManager.UpdateAsync(
            id, input.SupervisedComputerId,
            input.Name, input.IsHidden, input.IsImportant
            );

            return ObjectMapper.Map<KnownProcess, KnownProcessDto>(knownProcess);
        }
    }
}