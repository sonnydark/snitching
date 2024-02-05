namespace SnitcherPortal.Permissions;

public static class SnitcherPortalPermissions
{
    public const string GroupName = "SnitcherPortal";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class SupervisedComputers
    {
        public const string Default = GroupName + ".SupervisedComputers";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ActivityRecords
    {
        public const string Default = GroupName + ".ActivityRecords";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class SnitchingLogs
    {
        public const string Default = GroupName + ".SnitchingLogs";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}