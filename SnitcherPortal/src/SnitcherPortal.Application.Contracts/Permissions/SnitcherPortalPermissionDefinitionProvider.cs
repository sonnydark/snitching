using SnitcherPortal.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace SnitcherPortal.Permissions;

public class SnitcherPortalPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SnitcherPortalPermissions.GroupName);

        myGroup.AddPermission(SnitcherPortalPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(SnitcherPortalPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(SnitcherPortalPermissions.MyPermission1, L("Permission:MyPermission1"));

        var supervisedComputerPermission = myGroup.AddPermission(SnitcherPortalPermissions.SupervisedComputers.Default, L("Permission:SupervisedComputers"));
        supervisedComputerPermission.AddChild(SnitcherPortalPermissions.SupervisedComputers.Create, L("Permission:Create"));
        supervisedComputerPermission.AddChild(SnitcherPortalPermissions.SupervisedComputers.Edit, L("Permission:Edit"));
        supervisedComputerPermission.AddChild(SnitcherPortalPermissions.SupervisedComputers.Delete, L("Permission:Delete"));

        var activityRecordPermission = myGroup.AddPermission(SnitcherPortalPermissions.ActivityRecords.Default, L("Permission:ActivityRecords"));
        activityRecordPermission.AddChild(SnitcherPortalPermissions.ActivityRecords.Create, L("Permission:Create"));
        activityRecordPermission.AddChild(SnitcherPortalPermissions.ActivityRecords.Edit, L("Permission:Edit"));
        activityRecordPermission.AddChild(SnitcherPortalPermissions.ActivityRecords.Delete, L("Permission:Delete"));

        var snitchingLogPermission = myGroup.AddPermission(SnitcherPortalPermissions.SnitchingLogs.Default, L("Permission:SnitchingLogs"));
        snitchingLogPermission.AddChild(SnitcherPortalPermissions.SnitchingLogs.Create, L("Permission:Create"));
        snitchingLogPermission.AddChild(SnitcherPortalPermissions.SnitchingLogs.Edit, L("Permission:Edit"));
        snitchingLogPermission.AddChild(SnitcherPortalPermissions.SnitchingLogs.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SnitcherPortalResource>(name);
    }
}