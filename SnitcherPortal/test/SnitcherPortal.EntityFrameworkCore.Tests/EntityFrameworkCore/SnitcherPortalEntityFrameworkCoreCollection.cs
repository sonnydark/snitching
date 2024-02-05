using Xunit;

namespace SnitcherPortal.EntityFrameworkCore;

[CollectionDefinition(SnitcherPortalTestConsts.CollectionDefinitionName)]
public class SnitcherPortalEntityFrameworkCoreCollection : ICollectionFixture<SnitcherPortalEntityFrameworkCoreFixture>
{

}
