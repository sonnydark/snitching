using Volo.Abp.Modularity;

namespace SnitcherPortal;

public abstract class SnitcherPortalApplicationTestBase<TStartupModule> : SnitcherPortalTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
