using SnitcherPortal.Samples;
using Xunit;

namespace SnitcherPortal.EntityFrameworkCore.Applications;

[Collection(SnitcherPortalTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<SnitcherPortalEntityFrameworkCoreTestModule>
{

}
