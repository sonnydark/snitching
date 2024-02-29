using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using static EtoDefinitions;

namespace SnitcherPortal.Hubs;

public class SnitchedClientHubEventHandler : ILocalEventHandler<KillCommandEto>, ILocalEventHandler<ShowMessageEto>, ILocalEventHandler<SetConfigurationEto>, ITransientDependency
{
    private readonly ILogger<SnitchedClientHubEventHandler> _logger;
    private readonly IHubContext<SnitchedClientHub, ISnitchedClientHubClient> _hubContext;

    public SnitchedClientHubEventHandler(
        ILogger<SnitchedClientHubEventHandler> logger,
        IHubContext<SnitchedClientHub, ISnitchedClientHubClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task HandleEventAsync(KillCommandEto eventData)
    {
        try
        {
            await _hubContext.Clients.Client(eventData.ConnectionId!).KillCommandReceived(eventData);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
    }

    public async Task HandleEventAsync(ShowMessageEto eventData)
    {
        try
        {
            await _hubContext.Clients.Client(eventData.ConnectionId!).ShowMessageReceived(eventData);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
    }

    public async Task HandleEventAsync(SetConfigurationEto eventData)
    {
        try
        {
            await _hubContext.Clients.Client(eventData.ConnectionId!).SetConfiguration(eventData);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
    }
}