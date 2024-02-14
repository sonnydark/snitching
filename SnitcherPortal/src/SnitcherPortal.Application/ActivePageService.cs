using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SnitcherPortal.Engine;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SnitcherPortal;

public class ActivePageService : ITransientDependency, IAsyncDisposable
{
    private readonly ILogger<ActivePageService> Logger;

    private HubConnection? HubConnection;
    private bool IsDisposed;

    public event EventHandler<DashboardDataDto>? OnDashboardChanged;

    private bool Initialized = false;

    public ActivePageService(ILogger<ActivePageService> logger)
    {
        this.Logger = logger;
    }

    /// <summary>
    /// Set event required handlers before calling initialize
    /// </summary>
    /// <param name="baseUri"></param>
    /// <returns></returns>
    public async Task InitializeAsync(string baseUri, string specificLog)
    {
        this.IsDisposed = false;
        if (this.OnDashboardChanged == null)
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
            .WithUrl(baseUri?.TrimEnd('/') + "/Kanban-hub",
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
                this.Logger.LogWarning(ex, "[Standard page function: {function}, by user {user}]: {logString}", $"ActivePageService.HubConnection.Closed", specificLog, "Reconnect will follow ..");
                Initialized = false;
                await this.InitializeAsync(baseUri, specificLog);
            }
        };

        if (this.OnDashboardChanged != null)
        {
            this.HubConnection.On<DashboardDataDto>("DashboardChanged", eto =>
            {
                this.OnDashboardChanged.Invoke(EventArgs.Empty, eto);
            });
        }

        try
        {
            await this.HubConnection.StartAsync();
            this.Initialized = true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "[Standard page function: {function}, by user {user}]: {logString}", $"{this.GetType()}.HubConnection.StartAsync", specificLog, "Retry attempts will follow ..");

            var tmrOnce = new System.Timers.Timer();
            tmrOnce.Elapsed += async (sender, args) =>
            {
                tmrOnce.Dispose();
                await this.InitializeAsync(baseUri, specificLog);
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