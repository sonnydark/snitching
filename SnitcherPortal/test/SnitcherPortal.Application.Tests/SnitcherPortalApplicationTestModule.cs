using Volo.Abp.Modularity;

namespace SnitcherPortal;

[DependsOn(
    typeof(SnitcherPortalApplicationModule),
    typeof(SnitcherPortalDomainTestModule)
)]
public class SnitcherPortalApplicationTestModule : AbpModule
{

}
