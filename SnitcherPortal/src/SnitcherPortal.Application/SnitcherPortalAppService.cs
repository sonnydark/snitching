using SnitcherPortal.Localization;
using Volo.Abp.Application.Services;

namespace SnitcherPortal;

/* Inherit your application services from this class.
 */
public abstract class SnitcherPortalAppService : ApplicationService
{
    protected SnitcherPortalAppService()
    {
        LocalizationResource = typeof(SnitcherPortalResource);
    }
}
