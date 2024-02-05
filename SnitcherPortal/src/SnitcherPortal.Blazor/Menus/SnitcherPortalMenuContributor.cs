using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
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

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                SnitcherPortalMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 1
            )
        );

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                SnitcherPortalMenus.HostDashboard,
                l["Menu:Dashboard"],
                "~/HostDashboard",
                icon: "fa fa-line-chart",
                order: 2
            ).RequirePermissions(SnitcherPortalPermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                SnitcherPortalMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "~/Dashboard",
                icon: "fa fa-line-chart",
                order: 2
            ).RequirePermissions(SnitcherPortalPermissions.Dashboard.Tenant)
        );

        /* Example nested menu definition:

        context.Menu.AddItem(
            new ApplicationMenuItem("Menu0", "Menu Level 0")
            .AddItem(new ApplicationMenuItem("Menu0.1", "Menu Level 0.1", url: "/test01"))
            .AddItem(
                new ApplicationMenuItem("Menu0.2", "Menu Level 0.2")
                    .AddItem(new ApplicationMenuItem("Menu0.2.1", "Menu Level 0.2.1", url: "/test021"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.2", "Menu Level 0.2.2")
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.1", "Menu Level 0.2.2.1", "/test0221"))
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.2", "Menu Level 0.2.2.2", "/test0222"))
                    )
                    .AddItem(new ApplicationMenuItem("Menu0.2.3", "Menu Level 0.2.3", url: "/test023"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.4", "Menu Level 0.2.4", url: "/test024")
                        .AddItem(new ApplicationMenuItem("Menu0.2.4.1", "Menu Level 0.2.4.1", "/test0241"))
                )
                .AddItem(new ApplicationMenuItem("Menu0.2.5", "Menu Level 0.2.5", url: "/test025"))
            )
            .AddItem(new ApplicationMenuItem("Menu0.2", "Menu Level 0.2", url: "/test02"))
        );

        */

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
        return Task.CompletedTask;
    }
}