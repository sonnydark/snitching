namespace SnitcherAgent.Services;

public static class HeartBeatService
{
    public static async Task HeartBeatTask()
    {
        try
        {
            var snitchingDataEto = FeatureService.GetSnitchingData();
            await AppDomain.Instance.SendSnitchingDataAsync(snitchingDataEto);
        }
        catch (Exception ex)
        {
            AppDomain.Instance.Logs.Add(ex.ToString());
        }

        await Task.Delay(AppDomain.Instance.Configuration!.Heartbeat);
        await Task.Run(HeartBeatTask);
    }
}
