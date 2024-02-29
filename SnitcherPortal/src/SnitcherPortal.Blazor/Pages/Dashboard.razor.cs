using Blazorise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SnitcherPortal.Blazor.Pages;

public partial class Dashboard
{
    public List<string> ComputerNames { get; set; } = [];
    public DashboardDataDto Model { get; set; } = new DashboardDataDto();

    public event EventHandler<DashboardDataDto>? OnDashboardChanged;
    private string? SelectedComputer { get; set; }

    private string? MessageText { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.ComputerNames = await this.SupervisedComputersAppService.GetAvailableComputersAsync();
        if (this.ComputerNames.Count > 0)
        {
            this.SelectedComputer = this.ComputerNames.First();
        }
        this.ActivePageService.OnDashboardChanged += DashboardChanged;
        await this.ActivePageService.InitializeAsync(this.NavigationManager.BaseUri, "");
        await ReloadPageAsync();
    }

    private async Task ReloadPageAsync()
    {
        if (!this.SelectedComputer.IsNullOrEmpty())
        {
            this.Model = await this.SupervisedComputersAppService.GetDashboardDataAsync(this.SelectedComputer);
        }
        this.StateHasChanged();
    }

    private async Task DropdownClicked(string computerName)
    {
        this.SelectedComputer = computerName;
        await ReloadPageAsync();
    }

    private async void DashboardChanged(object? sender, DashboardDataDto eventDto)
    {
        await InvokeAsync(() =>
        {
            if (this.IsDisposed == true || eventDto == null || eventDto?.ComputerName != this.SelectedComputer)
            {
                return;
            }

            this.Model = eventDto!;
            StateHasChanged();
        });
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Timer timer = new(1000);
            timer.Elapsed += new ElapsedEventHandler((object? sender, ElapsedEventArgs e) =>
            {
                if (this.Model?.LastUpdate.HasValue == true)
                {
                    InvokeAsync(() => { this.Model.LastUpdate.Value.AddSeconds(1); this.StateHasChanged(); });
                }
            });
            timer.Enabled = true;
        }
    }

    public async Task ProcessButtonClickedAsync(string processName)
    {
        bool success = false;
        try
        {
            await this.SupervisedComputersAppService.KillProcessAsync(this.SelectedComputer!, processName);
            success = true;
        }
        catch (Exception ex)
        {
            await this.HandleErrorAsync(ex);
        }
        if (success)
        {
            this.Model.ProcessList.RemoveAll(e => e.ProcessName.Contains(processName));
            this.StateHasChanged();
        }
    }

    public async Task SendMessageAsync()
    {
        if (this.MessageText.IsNullOrWhiteSpace())
        {
            return;
        }
        try
        {
            await this.SupervisedComputersAppService.SendMessageAsync(this.SelectedComputer!, this.MessageText);
        }
        catch (Exception ex)
        {
            await this.HandleErrorAsync(ex);
        }
    }
}
