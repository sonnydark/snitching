using Volo.Abp.Modularity;

namespace SnitcherPortal;

[DependsOn(
    typeof(SnitcherPortalDomainModule),
    typeof(SnitcherPortalTestBaseModule)
)]
public class SnitcherPortalDomainTestModule : AbpModule
{

}
