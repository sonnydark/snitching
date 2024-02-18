using SnitcherAgent.Services;
using System.Windows;
using static SnitcherAgent.SignalREto.EtoDefinitions;

namespace SnitcherAgent;

public class AppDomain
{
    private static AppDomain? _instance;
    private static readonly object lockObject = new();

    public List<string> Logs = [];

    private readonly SignalRConnetorService _signalRConnetorService = new();
    public SetConfigurationEto? Configuration { get; set; }

    private AppDomain()
    {
    }

    public static AppDomain Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    _instance ??= new AppDomain();
                }
            }
            return _instance;
        }
    }

    public async Task CreateSignalRConnection()
    {
        _signalRConnetorService.OnSetConfiguration += SetConfiguration;
        _signalRConnetorService.OnKillCommandReceived += KillCommandReceived;
        _signalRConnetorService.OnShowMessageReceived += ShowMessageReceived;
        _signalRConnetorService.OnHideMessageReceived += HideMessageReceived;

        var portalUrl = Environment.GetEnvironmentVariable(StartupPrompt.PortalUrlEnvVarName, EnvironmentVariableTarget.User);
        if (!string.IsNullOrEmpty(portalUrl))
        {
            await _signalRConnetorService.InitializeAsync(portalUrl);
        }
        else
        {
            Environment.Exit(0);
        }
    }

    private async void SetConfiguration(object? sender, SetConfigurationEto eto)
    {
        bool startHearthbeat = this.Configuration == null;
        this.Configuration = eto;
        if (startHearthbeat)
        {
            await Task.Run(HeartBeatService.HeartBeatTask);
        }
    }

    private void KillCommandReceived(object? sender, KillCommandEto eto)
    {
        if (eto?.Processes != null)
        {
            FeatureService.KillProcess(eto.Processes);
        }
    }

    private async void ShowMessageReceived(object? sender, ShowMessageEto eto)
    {
    }

    private async void HideMessageReceived(object? sender, HideMessageEto eto)
    {
    }

    public async Task SendSnitchingDataAsync(SnitchingDataEto data)
    {
        await _signalRConnetorService.SendSnitchingDataAsync(data);
    }
}
