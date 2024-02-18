using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace SnitcherPortal.Hubs;

[HubRoute("/Active-page-hub")]

public class ActivePageHub : AbpHub<IActivePageHubClient>
{
}

public interface IActivePageHubClient
{
    Task DashboardChanged(DashboardDataDto eto);
}