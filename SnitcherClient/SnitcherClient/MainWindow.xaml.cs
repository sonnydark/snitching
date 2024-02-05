using SnitcherClient.Business;
using SnitcherClient.Interface;
using SnitcherClient.Shared;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnitcherClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int RefreshPeriod = 1000;

        public MainWindow()
        {
            InitializeComponent();

            Task.Run(ProcessRenderingLoop);
        }

        private async Task ProcessRenderingLoop()
        {
            try
            {
                var data = await SnitcherClientFunctions.GetShitchingData();
                var filteredOutProcesses = Constants.FilteredOutProcesses.Split(",", StringSplitOptions.TrimEntries).ToList();
                var markedProcesses = Constants.MarkedProcesses.Split(",", StringSplitOptions.TrimEntries).ToList();
                data.Processes = data.Processes.Where(p => !filteredOutProcesses.Contains(p.ProcessName)).Distinct().ToList();

                Dispatcher.Invoke(() => {
                    this.ProcessList.Children.Clear();
                    foreach (var process in data.Processes.OrderBy(p => p.StartTime))
                    {
                        Button btn = new()
                        {
                            Tag = process.ProcessName,
                            Content = $"{process.StartTime?.ToString("HH:mm:ss")} - {process.ProcessName}",
                            Background = markedProcesses.Contains(process.ProcessName) ? Brushes.IndianRed : Brushes.Beige,
                        };
                        btn.Click += KillClick;
                        this.ProcessList.Children.Add(btn);
                    }

                    this.CurrentConfig.Text = JsonSerializer.Serialize(data.Config);
                    if (!(this.NewConfig.Text?.Length > 0))
                    {
                        this.NewConfig.Text = this.CurrentConfig.Text;
                    }
                });
            }
            catch (Exception ex)
            {
                AddToConsole(ex.ToString());
            }
            finally
            {
                Thread.Sleep(RefreshPeriod);
                await Task.Run(ProcessRenderingLoop);
            }
        }

        private void AddToConsole(string line)
        {
            Dispatcher.Invoke(() =>
            {
                var newLine = $"{DateTime.Now.ToString("HH:mm:ss")}: {line}{Environment.NewLine}";
                this.Console.Text = newLine + this.Console.Text;
            });
        }

        private async void KillClick(object sender, RoutedEventArgs e)
        {
            var processName = ((Button)e.Source).Tag as string;
            await SnitcherClientFunctions.KillProcess(processName);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SnitcherClientFunctions.Configure(JsonSerializer.Deserialize<RuntimeConfigDto>(this.NewConfig.Text));
            this.NewConfig.Text = "";
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Task.Run(ProcesskillingLoop);
        }

        private async Task ProcesskillingLoop()
        {
            try
            {
                var markedProcesses = Constants.MarkedProcesses.Split(",", StringSplitOptions.TrimEntries).ToList();
                foreach (var markedProcess in markedProcesses)
                {
                    await SnitcherClientFunctions.KillProcess(markedProcess);
                }
                Thread.Sleep(60 * 1000);
            }
            catch (Exception ex)
            {
                AddToConsole(ex.ToString());
            }
            finally
            {
                Thread.Sleep(RefreshPeriod);
                await Task.Run(ProcesskillingLoop);
            }
        }
    }
}
