using SnitcherServer.Interface;
using System.Diagnostics;

namespace SnitcherServer.Services;

public static class NastyStuffService
{
    public static List<RunningProcessDto> GetProcesses()
    {
        var result = new List<RunningProcessDto>();
        var processes = Process.GetProcesses().ToList();

        foreach (var process in processes)
        {
            var runningProcessDto = new RunningProcessDto()
            {
                ProcessName = process.ProcessName,
            };

            try
            {
                runningProcessDto.StartTime = process.StartTime;
            }
            catch (Exception) { }

            result.Add(runningProcessDto);
        }

        return result;
    }

    public static void KillProcess(string processName)
    {
        var process = Process.GetProcesses().Where(p => p.ProcessName == processName).FirstOrDefault();
        process?.Kill();
    }
}
