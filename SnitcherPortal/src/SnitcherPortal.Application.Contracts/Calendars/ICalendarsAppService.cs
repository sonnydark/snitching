using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SnitcherPortal.Calendars
{
    public interface ICalendarsAppService : IApplicationService
    {
        Task<PagedResultDto<CalendarDto>> GetListBySupervisedComputerIdAsync(GetCalendarListInput input);

        Task<PagedResultDto<CalendarDto>> GetListAsync(GetCalendarsInput input);

        Task<CalendarDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<CalendarDto> CreateAsync(CalendarCreateDto input);

        Task<CalendarDto> UpdateAsync(Guid id, CalendarUpdateDto input);
    }
}