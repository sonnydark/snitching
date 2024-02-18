using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using static SnitcherAgent.SignalREto.EtoDefinitions;

namespace SnitcherAgent.Services;

public class SignalRConnetorService : IAsyncDisposable
{
    private HubConnection? HubConnection;
    private bool IsDisposed;

    public event EventHandler<SetConfigurationEto>? OnSetConfiguration;
    public event EventHandler<KillCommandEto>? OnKillCommandReceived;
    public event EventHandler<ShowMessageEto>? OnShowMessageReceived;
    public event EventHandler<HideMessageEto>? OnHideMessageReceived;

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
                AppDomain.Instance.Logs.Add("HubConnection.Closed, Reconnect will follow ..");
                this.Initialized = false;
                await this.InitializeAsync(baseUri!);
            }
        };

        // Events registration
        if (this.OnSetConfiguration != null)
        {
            this.HubConnection.On<SetConfigurationEto>("SetConfiguration", eto =>
            {
                this.OnSetConfiguration.Invoke(EventArgs.Empty, eto);
            });
        }

        if (this.OnKillCommandReceived != null)
        {
            this.HubConnection.On<KillCommandEto>("KillCommandReceived", eto =>
            {
                this.OnKillCommandReceived.Invoke(EventArgs.Empty, eto);
            });
        }

        if (this.OnShowMessageReceived != null)
        {
            this.HubConnection.On<ShowMessageEto>("ShowMessageReceived", eto =>
            {
                this.OnShowMessageReceived.Invoke(EventArgs.Empty, eto);
            });
        }

        if (this.OnHideMessageReceived != null)
        {
            this.HubConnection.On<HideMessageEto>("HideMessageReceived", eto =>
            {
                this.OnHideMessageReceived.Invoke(EventArgs.Empty, eto);
            });
        }

        try
        {
            await this.HubConnection.StartAsync();
            this.Initialized = true;
        }
        catch (Exception ex)
        {
            AppDomain.Instance.Logs.Add($"Exception occured: {ex.ToString()}, Reconnect will follow ..");

            var tmrOnce = new System.Timers.Timer();
            tmrOnce.Elapsed += async (sender, args) =>
            {
                tmrOnce.Dispose();
                await this.InitializeAsync(baseUri!);
            };

            tmrOnce.Interval = 5000;
            tmrOnce.Start();
        }
    }

    public async Task SendSnitchingDataAsync(SnitchingDataEto data)
    {
        if (this.IsDisposed == true)
        {
            return;
        }

        if (!this.Initialized)
        {
            AppDomain.Instance.Logs.Add($"Hub not initialised, data will not be transfered to the portal.");
        }

        await this.HubConnection!.InvokeAsync("ReceiveSnitchingData", data);
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
