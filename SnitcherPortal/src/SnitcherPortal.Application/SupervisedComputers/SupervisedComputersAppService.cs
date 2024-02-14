using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using SnitcherPortal.Permissions;
using SnitcherPortal.Calendars;
using SnitcherPortal.Engine;
using SnitcherPortal.ActivityRecords;
using System.Text.Json;

namespace SnitcherPortal.SupervisedComputers
{

    [Authorize(SnitcherPortalPermissions.SupervisedComputers.Default)]
    public class SupervisedComputersAppService : ApplicationService, ISupervisedComputersAppService
    {

        protected ISupervisedComputerRepository _supervisedComputerRepository;
        protected IActivityRecordRepository _activityRecordRepository;
        protected SupervisedComputerManager _supervisedComputerManager;
        protected SnitcherClientFunctions _snitcherClientFunctions;
        protected CalendarManager _calendarManager;

        public SupervisedComputersAppService(ISupervisedComputerRepository supervisedComputerRepository,
            IActivityRecordRepository activityRecordRepository,
            SupervisedComputerManager supervisedComputerManager,
            SnitcherClientFunctions snitcherClientFunctions,
            CalendarManager calendarManager)
        {
            _calendarManager = calendarManager;
            _supervisedComputerRepository = supervisedComputerRepository;
            _activityRecordRepository = activityRecordRepository;
            _snitcherClientFunctions = snitcherClientFunctions;
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

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = false
            };

            var calendarDefaultsWorkingDays = JsonSerializer.Serialize(new CalendarSettingsJson()
            {
                Quota = 5,
                Hours =
                [
                    new()
                    {
                        Start = new TimeSpan(13, 0, 0),
                        End = new TimeSpan(20, 0, 0)
                    }
                ]
            }, options);

            var calendarDefaultsWeekend = JsonSerializer.Serialize(new CalendarSettingsJson()
            {
                Quota = 8,
                Hours =
                [
                    new()
                    {
                        Start = new TimeSpan(10, 0, 0),
                        End = new TimeSpan(20, 0, 0)
                    }
                ]
            }, options);

            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Monday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Tuesday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Wednesday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Thursday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Friday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Saturday, calendarDefaultsWeekend);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Sunday, calendarDefaultsWeekend);

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
            var supervisedComputer = (await _supervisedComputerRepository.GetQueryableNoTrackingAsync(true)).First(sc => sc.Name == computerName);
            var activityRecords = (await _activityRecordRepository.GetQueryableNoTrackingAsync(false))
                .OrderByDescending(e => e.StartTime).Take(10).ToList();
            return DashboardMapper.Map(supervisedComputer, activityRecords, null);
        }

        public async Task KillProcessAsync(string computerName, string processName)
        {
            var supervisedComputer = (await _supervisedComputerRepository.GetQueryableNoTrackingAsync(false)).First(sc => sc.Name == computerName);
            await _snitcherClientFunctions.KillProcesses(supervisedComputer.IpAddress!, [processName]);

            var updatedActivity = (await _activityRecordRepository.GetQueryableAsync()).FirstOrDefault(e => e.EndTime == null);
            if (updatedActivity != null)
            {
                updatedActivity.Data += updatedActivity.Data.IsNullOrWhiteSpace() == false ? ", " : "";
                updatedActivity.Data += $"'{processName}' killed manually at {DateTime.Now.ToString("HH:mm:ss")}";
                await _activityRecordRepository.UpdateAsync(updatedActivity);
            }
        }
    }
}