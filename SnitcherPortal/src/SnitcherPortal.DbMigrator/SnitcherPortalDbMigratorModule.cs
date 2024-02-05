using SnitcherPortal.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SnitcherPortal.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SnitcherPortalEntityFrameworkCoreModule),
    typeof(SnitcherPortalApplicationContractsModule)
)]
public class SnitcherPortalDbMigratorModule : AbpModule
{
}
