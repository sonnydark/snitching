using System.Windows;

namespace SnitcherAgent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex? mutex;

        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string mutexName = "SnitcherAgent";
            mutex = new Mutex(true, mutexName, out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Another instance is already running.");
                Environment.Exit(0);
            }
            CheckPortalUrl();

            await AppDomain.Instance.CreateSignalRConnection(this.Dispatcher);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
        }

        private void CheckPortalUrl()
        {
            var portalUrl = Environment.GetEnvironmentVariable(StartupPrompt.PortalUrlEnvVarName, EnvironmentVariableTarget.User);
            if (!(portalUrl?.Length > 0))
            {
                var startupPrompt = new StartupPrompt();
                startupPrompt.ShowDialog();
            }
        }
    }
}
