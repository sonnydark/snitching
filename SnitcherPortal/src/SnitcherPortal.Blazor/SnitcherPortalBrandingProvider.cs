using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace SnitcherPortal.Blazor;

[Dependency(ReplaceServices = true)]
public class SnitcherPortalBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "SnitcherPortal";
}
