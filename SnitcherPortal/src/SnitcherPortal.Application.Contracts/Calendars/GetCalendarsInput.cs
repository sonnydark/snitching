using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.Calendars
{
    public class GetCalendarsInput : PagedAndSortedResultRequestDto
    {
        public Guid? SupervisedComputerId { get; set; }

        public string? FilterText { get; set; }

        public int? DayOfWeekMin { get; set; }
        public int? DayOfWeekMax { get; set; }
        public string? AllowedHours { get; set; }

        public GetCalendarsInput()
        {

        }
    }
}