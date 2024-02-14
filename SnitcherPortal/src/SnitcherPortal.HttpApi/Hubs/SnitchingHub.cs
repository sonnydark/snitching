using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace SnitcherPortal.Hubs;

[HubRoute("/Kanban-hub")]

public class SnitchingHub : AbpHub<ITransportRequestHubClient>
{
}

public interface ITransportRequestHubClient
{
    Task DashboardChanged(DashboardDataDto eto);
}