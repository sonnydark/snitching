namespace SnitcherPortal.KnownProcesses
{
    public static class KnownProcessConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "KnownProcess." : string.Empty);
        }

        public const int NameMinLength = 1;
        public const int NameMaxLength = 256;
    }
}