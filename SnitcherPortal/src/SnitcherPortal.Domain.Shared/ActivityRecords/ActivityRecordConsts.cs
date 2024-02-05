namespace SnitcherPortal.ActivityRecords
{
    public static class ActivityRecordConsts
    {
        private const string DefaultSorting = "{0}StartTime asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ActivityRecord." : string.Empty);
        }

    }
}