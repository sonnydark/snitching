using SnitcherPortal.Localization;
using Volo.Abp.AspNetCore.Components;

namespace SnitcherPortal.Blazor;

public abstract class SnitcherPortalComponentBase : AbpComponentBase
{
    protected SnitcherPortalComponentBase()
    {
        LocalizationResource = typeof(SnitcherPortalResource);
    }
}
