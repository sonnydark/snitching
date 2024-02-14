using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace SnitcherPortal.Hubs;

public class SnitchingHubEventHandler : ILocalEventHandler<DashboardDataDto>, ITransientDependency
{
    private readonly ILogger<SnitchingHubEventHandler> _logger;
    private readonly IHubContext<SnitchingHub, ITransportRequestHubClient> _hubContext;

    public SnitchingHubEventHandler(
        ILogger<SnitchingHubEventHandler> logger,
        IHubContext<SnitchingHub, ITransportRequestHubClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task HandleEventAsync(DashboardDataDto eventData)
    {
        try
        {
            await _hubContext.Clients.All.DashboardChanged(eventData);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
    }
}