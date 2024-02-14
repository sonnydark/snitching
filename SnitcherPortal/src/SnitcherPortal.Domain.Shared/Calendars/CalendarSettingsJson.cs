using System;
using System.Collections.Generic;

namespace SnitcherPortal.Calendars;

public class CalendarSettingsJson
{
    public int Quota { get; set; }

    public List<AllowedHoursJson> Hours { get; set; } = [];
}

public class AllowedHoursJson
{
    public TimeSpan Start { get; set; }

    public TimeSpan End { get; set; }
}
