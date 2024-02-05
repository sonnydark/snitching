using SnitcherClient.Interface;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnitcherClient.Business;

public class SnitcherClientFunctions
{
    private static string SnitchedIp = "http://192.168.1.10:8080/";

    public static async Task<SnitchingDataDto> GetShitchingData()
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(SnitchedIp)
        };
        var response = await client.GetAsync("Snitching");
        return await response.Content.ReadFromJsonAsync<SnitchingDataDto>();
    }

    public static async Task KillProcess(string processName)
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(SnitchedIp)
        };
        await client.PutAsync($"Snitching?processName={processName}", null);
    }

    public static async Task Configure(RuntimeConfigDto config)
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(SnitchedIp)
        };
        HttpContent httpContent = new StringContent(JsonSerializer.Serialize(config), Encoding.UTF8, "application/json");
        await client.PutAsync($"Snitching/configure", httpContent);
    }
}
