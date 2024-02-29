using Microsoft.Extensions.Logging;
using SnitcherPortal.SupervisedComputers;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.EventBus.Local;
using static EtoDefinitions;

namespace SnitcherPortal.Hubs;

[HubRoute("/Snitched-client-hub")]

public class SnitchedClientHub : AbpHub<ISnitchedClientHubClient>
{
    private readonly ILogger<SnitchedClientHub> _logger;
    private readonly ILocalEventBus _localEventBus;
    private readonly ISupervisedComputersAppService _supervisedComputersAppService;

    public SnitchedClientHub(ILogger<SnitchedClientHub> logger,
        ILocalEventBus localEventBus,
        ISupervisedComputersAppService supervisedComputersAppService)
    {
        _logger = logger;
        _localEventBus = localEventBus;
        _supervisedComputersAppService = supervisedComputersAppService;
    }

    public override async Task OnConnectedAsync()
    {   
        string connectionId = Context.ConnectionId;
        await Clients.Client(connectionId).SetConfiguration(new SetConfigurationEto()
        {
            ConnectionId = Context.ConnectionId,
            Heartbeat = 1000
        });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        string connectionId = Context.ConnectionId;
        try
        {
            await _localEventBus.PublishAsync(new ClientDisconnectEto()
            {
                ConnectionId = connectionId,
                Message = ex?.ToString() ?? "Disconnected for unknown reason"
            }, false);
        }
        catch (Exception e)
        {
            _logger.LogException(e);
        }
        await base.OnDisconnectedAsync(ex);
    }

    // Theese are messages received from client
    public async Task ReceiveSnitchingData(SnitchingDataEto eto)
    {
        try
        {
            await _localEventBus.PublishAsync(eto, false);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
    }
}

public interface ISnitchedClientHubClient
{
    Task SetConfiguration(SetConfigurationEto eto);
    Task KillCommandReceived(KillCommandEto eto);
    Task ShowMessageReceived(ShowMessageEto eto);
}