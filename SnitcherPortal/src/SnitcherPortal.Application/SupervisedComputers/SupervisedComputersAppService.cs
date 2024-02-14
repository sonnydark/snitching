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
using SnitcherPortal.Calendars;

namespace SnitcherPortal.SupervisedComputers
{

    [Authorize(SnitcherPortalPermissions.SupervisedComputers.Default)]
    public class SupervisedComputersAppService : ApplicationService, ISupervisedComputersAppService
    {

        protected ISupervisedComputerRepository _supervisedComputerRepository;
        protected SupervisedComputerManager _supervisedComputerManager;
        protected CalendarManager _calendarManager;

        public SupervisedComputersAppService(ISupervisedComputerRepository supervisedComputerRepository,
            SupervisedComputerManager supervisedComputerManager,
            CalendarManager calendarManager)
        {
            _calendarManager = calendarManager;
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
            supervisedComputer.Status = SupervisedComputerStatus.OFFLINE;

            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Monday);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Tuesday);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Wednesday);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Thursday);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Friday);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Saturday);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Sunday);

            return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(supervisedComputer);
        }

        [Authorize(SnitcherPortalPermissions.SupervisedComputers.Edit)]
        public virtual async Task<SupervisedComputerDto> UpdateAsync(Guid id, SupervisedComputerUpdateDto input)
        {
            var supervisedComputer = await _supervisedComputerManager.UpdateAsync(
            id,
            input.Name, input.Identifier, input.IsCalendarActive, input.Status, input.IpAddress, input.BanUntil, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(supervisedComputer);
        }

        public async Task<List<string>> GetAvailableComputersAsync()
        {
            return (await _supervisedComputerRepository.GetQueryableNoTrackingAsync())
                .Select(e => e.Name).ToList();
        }

        public async Task<DashboardDataDto> GetDashboardDataAsync(string computerName)
        {
            var supervisedComputer = (await _supervisedComputerRepository.WithDetailsAsync()).Where(sc => sc.Name == computerName).First();
            return null;
        }
    }
}