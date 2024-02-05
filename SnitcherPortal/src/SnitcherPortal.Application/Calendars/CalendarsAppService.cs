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
using SnitcherPortal.Calendars;

namespace SnitcherPortal.Calendars
{

    [Authorize(SnitcherPortalPermissions.Calendars.Default)]
    public class CalendarsAppService : ApplicationService, ICalendarsAppService
    {

        protected ICalendarRepository _calendarRepository;
        protected CalendarManager _calendarManager;

        public CalendarsAppService(ICalendarRepository calendarRepository, CalendarManager calendarManager)
        {

            _calendarRepository = calendarRepository;
            _calendarManager = calendarManager;
        }

        public virtual async Task<PagedResultDto<CalendarDto>> GetListBySupervisedComputerIdAsync(GetCalendarListInput input)
        {
            var calendars = await _calendarRepository.GetListBySupervisedComputerIdAsync(
                input.SupervisedComputerId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<CalendarDto>
            {
                TotalCount = await _calendarRepository.GetCountBySupervisedComputerIdAsync(input.SupervisedComputerId),
                Items = ObjectMapper.Map<List<Calendar>, List<CalendarDto>>(calendars)
            };
        }

        public virtual async Task<PagedResultDto<CalendarDto>> GetListAsync(GetCalendarsInput input)
        {
            var totalCount = await _calendarRepository.GetCountAsync(input.FilterText, input.DayOfWeekMin, input.DayOfWeekMax, input.AllowedHours);
            var items = await _calendarRepository.GetListAsync(input.FilterText, input.DayOfWeekMin, input.DayOfWeekMax, input.AllowedHours, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CalendarDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Calendar>, List<CalendarDto>>(items)
            };
        }

        public virtual async Task<CalendarDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Calendar, CalendarDto>(await _calendarRepository.GetAsync(id));
        }

        [Authorize(SnitcherPortalPermissions.Calendars.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _calendarRepository.DeleteAsync(id);
        }

        [Authorize(SnitcherPortalPermissions.Calendars.Create)]
        public virtual async Task<CalendarDto> CreateAsync(CalendarCreateDto input)
        {

            var calendar = await _calendarManager.CreateAsync(input.SupervisedComputerId,
            input.DayOfWeek, input.AllowedHours
            );

            return ObjectMapper.Map<Calendar, CalendarDto>(calendar);
        }

        [Authorize(SnitcherPortalPermissions.Calendars.Edit)]
        public virtual async Task<CalendarDto> UpdateAsync(Guid id, CalendarUpdateDto input)
        {

            var calendar = await _calendarManager.UpdateAsync(
            id, input.SupervisedComputerId,
            input.DayOfWeek, input.AllowedHours
            );

            return ObjectMapper.Map<Calendar, CalendarDto>(calendar);
        }
    }
}