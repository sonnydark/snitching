using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace SnitcherPortal.Calendars
{
    public class CalendarDto : EntityDto<Guid>
    {
        public Guid SupervisedComputerId { get; set; }
        public int DayOfWeek { get; set; }
        public string? AllowedHours { get; set; }

    }
}