using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;
using static SnitcherAgent.SignalREto.EtoDefinitions;

namespace SnitcherAgent
{
    /// <summary>
    /// Interaction logic for StartupPrompt.xaml
    /// </summary>
    public partial class MessageSnitch : Window
    {
        public MessageSnitch(ShowMessageEto eto)
        {
            InitializeComponent();
            this.MessageTextBox.Text = eto.Message;

            System.Timers.Timer timer = new(eto.Duration * 1000);
            timer.Elapsed += new ElapsedEventHandler((object? sender, ElapsedEventArgs e) =>
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    timer.Enabled = false;
                    this.Hide();
                });
            });
            timer.Enabled = true;
        }
    }
}