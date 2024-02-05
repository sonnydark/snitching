namespace SnitcherServer.Services;

public class WatchdogService : IHostedService
{
    public WatchdogService()
    {
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(DoBackgroundTask);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void DoBackgroundTask()
    {
        var cfg = RuntimeConfigService.GetConfig();
        if (cfg.AutokillProcesses?.Count > 0)
        {
            foreach (var process in cfg.AutokillProcesses)
            {
                NastyStuffService.KillProcess(process);
            }
        }
        Task.Delay(cfg.AutokillInterval).Wait();
        Task.Run(DoBackgroundTask);
    }
}