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
using SnitcherPortal.SnitchingLogs;

namespace SnitcherPortal.SnitchingLogs
{

    [Authorize(SnitcherPortalPermissions.SnitchingLogs.Default)]
    public class SnitchingLogsAppService : ApplicationService, ISnitchingLogsAppService
    {

        protected ISnitchingLogRepository _snitchingLogRepository;
        protected SnitchingLogManager _snitchingLogManager;

        public SnitchingLogsAppService(ISnitchingLogRepository snitchingLogRepository, SnitchingLogManager snitchingLogManager)
        {

            _snitchingLogRepository = snitchingLogRepository;
            _snitchingLogManager = snitchingLogManager;
        }

        public virtual async Task<PagedResultDto<SnitchingLogDto>> GetListBySupervisedComputerIdAsync(GetSnitchingLogListInput input)
        {
            var snitchingLogs = await _snitchingLogRepository.GetListBySupervisedComputerIdAsync(
                input.SupervisedComputerId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<SnitchingLogDto>
            {
                TotalCount = await _snitchingLogRepository.GetCountBySupervisedComputerIdAsync(input.SupervisedComputerId),
                Items = ObjectMapper.Map<List<SnitchingLog>, List<SnitchingLogDto>>(snitchingLogs)
            };
        }

        public virtual async Task<PagedResultDto<SnitchingLogDto>> GetListAsync(GetSnitchingLogsInput input)
        {
            var totalCount = await _snitchingLogRepository.GetCountAsync(input.FilterText, input.TimestampMin, input.TimestampMax, input.Message);
            var items = await _snitchingLogRepository.GetListAsync(input.FilterText, input.TimestampMin, input.TimestampMax, input.Message, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SnitchingLogDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SnitchingLog>, List<SnitchingLogDto>>(items)
            };
        }

        public virtual async Task<SnitchingLogDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SnitchingLog, SnitchingLogDto>(await _snitchingLogRepository.GetAsync(id));
        }

        [Authorize(SnitcherPortalPermissions.SnitchingLogs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _snitchingLogRepository.DeleteAsync(id);
        }

        [Authorize(SnitcherPortalPermissions.SnitchingLogs.Create)]
        public virtual async Task<SnitchingLogDto> CreateAsync(SnitchingLogCreateDto input)
        {

            var snitchingLog = await _snitchingLogManager.CreateAsync(input.SupervisedComputerId,
            input.Timestamp, input.Message
            );

            return ObjectMapper.Map<SnitchingLog, SnitchingLogDto>(snitchingLog);
        }

        [Authorize(SnitcherPortalPermissions.SnitchingLogs.Edit)]
        public virtual async Task<SnitchingLogDto> UpdateAsync(Guid id, SnitchingLogUpdateDto input)
        {

            var snitchingLog = await _snitchingLogManager.UpdateAsync(
            id, input.SupervisedComputerId,
            input.Timestamp, input.Message
            );

            return ObjectMapper.Map<SnitchingLog, SnitchingLogDto>(snitchingLog);
        }
    }
}