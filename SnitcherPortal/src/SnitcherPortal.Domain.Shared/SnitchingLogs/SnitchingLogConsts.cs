namespace SnitcherPortal.SnitchingLogs
{
    public static class SnitchingLogConsts
    {
        private const string DefaultSorting = "{0}Timestamp asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SnitchingLog." : string.Empty);
        }

    }
}