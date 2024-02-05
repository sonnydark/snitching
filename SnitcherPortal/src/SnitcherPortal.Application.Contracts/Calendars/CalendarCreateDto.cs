using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnitcherPortal.Calendars
{
    public class CalendarCreateDto
    {
        public Guid SupervisedComputerId { get; set; }
        public int DayOfWeek { get; set; }
        public string? AllowedHours { get; set; }
    }
}