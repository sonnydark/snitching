using SnitcherPortal.Samples;
using Xunit;

namespace SnitcherPortal.EntityFrameworkCore.Domains;

[Collection(SnitcherPortalTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<SnitcherPortalEntityFrameworkCoreTestModule>
{

}
