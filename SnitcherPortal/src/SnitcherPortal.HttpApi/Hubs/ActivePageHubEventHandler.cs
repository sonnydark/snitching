using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace SnitcherPortal.Hubs;

public class ActivePageHubEventHandler : ILocalEventHandler<DashboardDataDto>, ITransientDependency
{
    private readonly ILogger<ActivePageHubEventHandler> _logger;
    private readonly IHubContext<ActivePageHub, IActivePageHubClient> _hubContext;

    public ActivePageHubEventHandler(
        ILogger<ActivePageHubEventHandler> logger,
        IHubContext<ActivePageHub, IActivePageHubClient> hubContext)
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