using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace SnitcherPortal.Calendars
{
    public class CalendarManager : DomainService
    {
        protected ICalendarRepository _calendarRepository;

        public CalendarManager(ICalendarRepository calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }

        public virtual async Task<Calendar> CreateAsync(
        Guid supervisedComputerId, int dayOfWeek, string? allowedHours = null)
        {

            var calendar = new Calendar(
             GuidGenerator.Create(),
             supervisedComputerId, dayOfWeek, allowedHours
             );

            return await _calendarRepository.InsertAsync(calendar);
        }

        public virtual async Task<Calendar> UpdateAsync(
            Guid id,
            Guid supervisedComputerId, int dayOfWeek, string? allowedHours = null
        )
        {

            var calendar = await _calendarRepository.GetAsync(id);

            calendar.SupervisedComputerId = supervisedComputerId;
            calendar.DayOfWeek = dayOfWeek;
            calendar.AllowedHours = allowedHours;

            return await _calendarRepository.UpdateAsync(calendar);
        }

    }
}