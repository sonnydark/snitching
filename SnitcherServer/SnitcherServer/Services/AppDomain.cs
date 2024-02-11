namespace SnitcherServer.Services;

public class AppDomain
{
    private static AppDomain? _instance;
    private static readonly object lockObject = new();

    public List<string> Logs = new();

    private AppDomain()
    {
    }

    public static AppDomain Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    _instance ??= new AppDomain();
                }
            }
            return _instance;
        }
    }

    
}