namespace SnitcherPortal.SupervisedComputers
{
    public static class SupervisedComputerConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SupervisedComputer." : string.Empty);
        }

        public const int NameMinLength = 1;
        public const int NameMaxLength = 64;
        public const int IdentifierMinLength = 1;
        public const int IdentifierMaxLength = 256;
        public const int ConnectionIdMaxLength = 64;
    }
}