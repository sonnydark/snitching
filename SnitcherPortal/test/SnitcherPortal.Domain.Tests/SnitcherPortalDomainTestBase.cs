using Volo.Abp.Modularity;

namespace SnitcherPortal;

/* Inherit from this class for your domain layer tests. */
public abstract class SnitcherPortalDomainTestBase<TStartupModule> : SnitcherPortalTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
