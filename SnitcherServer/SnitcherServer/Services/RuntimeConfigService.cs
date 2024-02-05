using SnitcherServer.Interface;
using System.Text.Json;

namespace SnitcherServer.Services;

public static class RuntimeConfigService
{
    public static RuntimeConfig GetConfig()
    {
        var json = GetConfigJson();
        return new RuntimeConfig()
        {
            AutokillInterval = int.Parse(json.AutokillInterval),
            AutokillProcesses = json.AutokillProcesses.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
        };
    }

    public static RuntimeConfigDto GetConfigJson()
    {
        return JsonSerializer.Deserialize<RuntimeConfigDto>(File.ReadAllText("RuntimeConfig.json"));
    }

    public static void SetConfig(RuntimeConfigDto config)
    {   
        File.WriteAllText("RuntimeConfig.json", JsonSerializer.Serialize(config));
    }
}

public class RuntimeConfig
{
    public int AutokillInterval { get; set; }
    public List<string> AutokillProcesses { get; set; }
}