using System.Threading.Tasks;
using SnitcherPortal.Localization;
using SnitcherPortal.Permissions;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Saas.Host.Blazor.Navigation;

namespace SnitcherPortal.Blazor.Menus;

public class SnitcherPortalMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<SnitcherPortalResource>();
        context.Menu.TryRemoveMenuItem(SaasHostMenus.GroupName);

        context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 4;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);

        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 2);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 3);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 4);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 5);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 6);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                SnitcherPortalMenus.SupervisedComputers,
                l["Menu:SupervisedComputers"],
                url: "/supervised-computers",
                icon: "fa fa-file-alt",
                requiredPermissionName: SnitcherPortalPermissions.SupervisedComputers.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                SnitcherPortalMenus.SupervisedComputers,
                l["Menu:Dashboard"],
                url: "/dashboard",
                icon: "fa fa-tablet-alt",
                requiredPermissionName: SnitcherPortalPermissions.SupervisedComputers.Default)
        );

        return Task.CompletedTask;
    }
}