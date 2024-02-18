using System.Diagnostics;
using static SnitcherAgent.SignalREto.EtoDefinitions;
using DeviceId;

namespace SnitcherAgent.Services;

public static class FeatureService
{
    public static SnitchingDataEto GetSnitchingData()
    {
        string? machineIdentifier = null;
        List<string>? processes = null;
        List<string>? logs = null;

        try
        {
            logs = AppDomain.Instance.Logs;
            AppDomain.Instance.Logs = [];
            machineIdentifier = new DeviceIdBuilder().AddMachineName().ToString();
            processes = Process.GetProcesses().ToList().Select(p => p.ProcessName).ToList();
        }
        catch (Exception ex)
        {
            logs ??= [];
            logs!.Add(ex.ToString());
        }

        return new SnitchingDataEto()
        {
            ConnectionId = AppDomain.Instance.Configuration!.ConnectionId,
            MachineIdentifier = machineIdentifier,
            Logs = logs,
            Processes = processes,
        };
    }

    public static bool KillProcess(List<string> processNames)
    {
        var procesList = Process.GetProcesses()
            .Where(p => processNames.Any(pn => pn.Contains(p.ProcessName, StringComparison.CurrentCultureIgnoreCase))).ToList();

        if (procesList.Count == 0)
        {
            return false;
        }

        foreach (var proces in procesList)
        {
            proces.Kill();
        }
        return true;
    }
}
