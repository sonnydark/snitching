using Microsoft.AspNetCore.SignalR.Client;
using SnitcherServer.Interface;

namespace SnitcherServer.Services;

public class SignalRConnetorService : IAsyncDisposable
{
    private HubConnection? HubConnection;
    private bool IsDisposed;

    public event EventHandler<CommandDto>? OnCommandReceived;

    private bool Initialized = false;

    public SignalRConnetorService()
    {
    }

    /// <summary>
    /// Set event required handlers before calling initialize
    /// </summary>
    /// <param name="baseUri"></param>
    /// <returns></returns>
    public async Task InitializeAsync(string baseUri)
    {
        this.IsDisposed = false;
        if (this.OnCommandReceived == null)
        {
            throw new Exception("Active page service is not initialized properly !! At least one event handler is needed.");
        }

        if (string.IsNullOrEmpty(baseUri))
        {
            throw new Exception("Active page service is not initialized properly !! Base URI is empty.");
        }

        if (this.Initialized == true)
        {
            return;
        }

        this.HubConnection = new HubConnectionBuilder()
            .WithUrl(baseUri?.TrimEnd('/') + "/Snitched-client-hub",
                options =>
                {
                    options.WebSocketConfiguration = conf =>
                    {
                        conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
                    };
                    options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
                    options.AccessTokenProvider = () => Task.FromResult(CancellationToken.None.ToString());
                })            
            .Build();

        this.HubConnection.Closed += async ex =>
        {
            if (!this.IsDisposed)
            {
                //this.Logger.LogWarning(ex, LogExtensions.BlazorPagePrefixTemplate, $"ActivePageService.HubConnection.Closed", specificLog, "Reconnect will follow ..");
                Initialized = false;
                await this.InitializeAsync(baseUri);
            }
        };

        // Events registration
        if (this.OnCommandReceived != null)
        {
            this.HubConnection.On<CommandDto>("CommandIssued", eto =>
            {
                this.OnCommandReceived.Invoke(EventArgs.Empty, eto);
            });
        }

        this.HubConnection.InvokeAsync

        try
        {
            await this.HubConnection.StartAsync();
            this.Initialized = true;
        }
        catch (Exception ex)
        {
            //this.Logger.LogError(ex, LogExtensions.BlazorPagePrefixTemplate, $"{this.GetType()}.HubConnection.StartAsync", specificLog, "Retry attempts will follow ..");

            var tmrOnce = new System.Timers.Timer();
            tmrOnce.Elapsed += async (sender, args) =>
            {
                tmrOnce.Dispose();
                await this.InitializeAsync(baseUri);
            };

            tmrOnce.Interval = 5000;
            tmrOnce.Start();
        }
    }

    public async ValueTask DisposeAsync()
    {
        this.IsDisposed = true;
        if (this.HubConnection != null)
        {
            await this.HubConnection.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
