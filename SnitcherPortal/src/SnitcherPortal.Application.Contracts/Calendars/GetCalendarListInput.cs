using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.Calendars
{
    public class GetCalendarListInput : PagedAndSortedResultRequestDto
    {
        public Guid SupervisedComputerId { get; set; }
    }
}