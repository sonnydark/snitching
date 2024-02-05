namespace SnitcherPortal.Calendars
{
    public static class CalendarConsts
    {
        private const string DefaultSorting = "{0}DayOfWeek asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Calendar." : string.Empty);
        }

    }
}