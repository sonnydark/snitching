using System.Windows;

namespace SnitcherAgent
{
    /// <summary>
    /// Interaction logic for StartupPrompt.xaml
    /// </summary>
    public partial class StartupPrompt : Window
    {
        public static string PortalUrlEnvVarName = "SnitcherPortalUrl";

        public StartupPrompt()
        {
            InitializeComponent();
            this.PortalUrlTextBox.Text = "https://192.168.1.16:44325";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable(PortalUrlEnvVarName, this.PortalUrlTextBox.Text, EnvironmentVariableTarget.User);
            this.Hide();
        }
    }
}