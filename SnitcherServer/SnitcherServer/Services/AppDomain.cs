using SnitcherServer.Interface;

namespace SnitcherServer.Services;

public class AppDomain
{
    private static AppDomain? _instance;
    private static readonly object lockObject = new();

    public List<string> Logs = new();

    private readonly SignalRConnetorService _signalRConnetorService = new();

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
        _signalRConnetorService.OnCommandReceived += CommandReceived;
        await _signalRConnetorService.InitializeAsync("https://192.168.1.16:44325");
    }

    private async void CommandReceived(object? sender, CommandDto eventDto)
    {
        NastyStuffService.KillProcess(new List<string> { eventDto.Test });
        //await InvokeAsync(async () =>
        //{
        //    if (this.IsDisposed == true || eventDto?.ComputerName != this.SelectedComputer)
        //    {
        //        return;
        //    }

        //    this.Model = eventDto;
        //    StateHasChanged();
        //});
    }
}