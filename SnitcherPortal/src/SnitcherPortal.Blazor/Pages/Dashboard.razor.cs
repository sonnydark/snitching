using Blazorise;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnitcherPortal.Blazor.Pages;

public partial class Dashboard
{
    public List<string> ComputerNames { get; set; } = [];
    public DashboardDataDto Model { get; set; } = new DashboardDataDto();

    public event EventHandler<DashboardDataDto>? OnDashboardChanged;
    private string SelectedComputer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.ComputerNames = await this.SupervisedComputersAppService.GetAvailableComputersAsync();

        this.ActivePageService.OnDashboardChanged += DashboardChanged;
    }

    private void DropdownClicked(string computerName)
    {
        this.SelectedComputer = computerName;
    }

    private async void DashboardChanged(object? sender, DashboardDataDto eventDto)
    {
        await InvokeAsync(async () =>
        {
            if (this.IsDisposed == true || eventDto?.ComputerName != this.SelectedComputer)
            {
                return;
            }

            this.Model = eventDto;
            StateHasChanged();
        });
    }

    public async Task ProcessButtonClickedAsync(string processName)
    {

    }
}
