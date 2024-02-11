using System.Diagnostics;

namespace SnitcherServer.Services;

public static class NastyStuffService
{
    public static List<string> GetProcesses()
    {
        return Process.GetProcesses().ToList().Select(p => p.ProcessName).ToList();
    }

    public static bool KillProcess(List<string> processNames)
    {
        var procesList = Process.GetProcesses()
            .Where(p => processNames.Any(pn => pn.ToLower().Contains(p.ProcessName.ToLower()))).ToList();

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
