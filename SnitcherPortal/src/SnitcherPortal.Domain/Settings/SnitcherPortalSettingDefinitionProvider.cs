using Volo.Abp.Settings;

namespace SnitcherPortal.Settings;

public class SnitcherPortalSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(SnitcherPortalSettings.MySetting1));
    }
}
