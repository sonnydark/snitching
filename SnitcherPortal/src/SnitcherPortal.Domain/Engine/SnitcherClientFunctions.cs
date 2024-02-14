using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.DependencyInjection;

namespace SnitcherPortal.Engine;

public class SnitcherClientFunctions : ITransientDependency
{
    protected IHttpClientFactory _httpClientFactory;

    public SnitcherClientFunctions(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<SnitchingDataDto?> GetShitchingData(string uri)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMilliseconds(3000);
        client.BaseAddress = new Uri(uri);
        var response = await client.GetAsync("Snitching/status");
        return await response.Content.ReadFromJsonAsync<SnitchingDataDto>();
    }

    public async Task KillProcesses(string uri, List<string> processNames)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMilliseconds(3000);
        client.BaseAddress = new Uri(uri);
        string parameters = string.Join("&", processNames.Select(e => $"processNames={e}"));
        await client.PutAsync($"Snitching/kill?{parameters}", null);
    }
}
