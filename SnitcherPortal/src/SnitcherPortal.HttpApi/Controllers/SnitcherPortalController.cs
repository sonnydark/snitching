using SnitcherPortal.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SnitcherPortal.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class SnitcherPortalController : AbpControllerBase
{
    protected SnitcherPortalController()
    {
        LocalizationResource = typeof(SnitcherPortalResource);
    }
}
