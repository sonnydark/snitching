using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using SnitcherPortal.SupervisedComputers;
using SnitcherPortal.Permissions;
using SnitcherPortal.Shared;
using SnitcherPortal.SnitchingLogs; 
using SnitcherPortal.ActivityRecords; 


namespace SnitcherPortal.Blazor.Pages
{
    public partial class SupervisedComputers
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<SupervisedComputerDto> SupervisedComputerList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSupervisedComputer { get; set; }
        private bool CanEditSupervisedComputer { get; set; }
        private bool CanDeleteSupervisedComputer { get; set; }
        private SupervisedComputerCreateDto NewSupervisedComputer { get; set; }
        private Validations NewSupervisedComputerValidations { get; set; } = new();
        private SupervisedComputerUpdateDto EditingSupervisedComputer { get; set; }
        private Validations EditingSupervisedComputerValidations { get; set; } = new();
        private Guid EditingSupervisedComputerId { get; set; }
        private Modal CreateSupervisedComputerModal { get; set; } = new();
        private Modal EditSupervisedComputerModal { get; set; } = new();
        private GetSupervisedComputersInput Filter { get; set; }
        private DataGridEntityActionsColumn<SupervisedComputerDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "supervisedComputer-create-tab";
        protected string SelectedEditTab = "supervisedComputer-edit-tab";
        private SupervisedComputerDto? SelectedSupervisedComputer;
        
        
                #region Child Entities
        
                #region SnitchingLogs

                private bool CanListSnitchingLog { get; set; }
                private bool CanCreateSnitchingLog { get; set; }
                private bool CanEditSnitchingLog { get; set; }
                private bool CanDeleteSnitchingLog { get; set; }
                private SnitchingLogCreateDto NewSnitchingLog { get; set; }
                private Dictionary<Guid, DataGrid<SnitchingLogDto>> SnitchingLogDataGrids { get; set; } = new();
                private int SnitchingLogPageSize { get; } = 5;
                private DataGridEntityActionsColumn<SnitchingLogDto> SnitchingLogEntityActionsColumns { get; set; } = new();
                private Validations NewSnitchingLogValidations { get; set; } = new();
                private Modal CreateSnitchingLogModal { get; set; } = new();
                private Guid EditingSnitchingLogId { get; set; }
                private SnitchingLogUpdateDto EditingSnitchingLog { get; set; }
                private Validations EditingSnitchingLogValidations { get; set; } = new();
                private Modal EditSnitchingLogModal { get; set; } = new();
    
            
                #endregion
        #region ActivityRecords

                private bool CanListActivityRecord { get; set; }
                private bool CanCreateActivityRecord { get; set; }
                private bool CanEditActivityRecord { get; set; }
                private bool CanDeleteActivityRecord { get; set; }
                private ActivityRecordCreateDto NewActivityRecord { get; set; }
                private Dictionary<Guid, DataGrid<ActivityRecordDto>> ActivityRecordDataGrids { get; set; } = new();
                private int ActivityRecordPageSize { get; } = 5;
                private DataGridEntityActionsColumn<ActivityRecordDto> ActivityRecordEntityActionsColumns { get; set; } = new();
                private Validations NewActivityRecordValidations { get; set; } = new();
                private Modal CreateActivityRecordModal { get; set; } = new();
                private Guid EditingActivityRecordId { get; set; }
                private ActivityRecordUpdateDto EditingActivityRecord { get; set; }
                private Validations EditingActivityRecordValidations { get; set; } = new();
                private Modal EditActivityRecordModal { get; set; } = new();
    
            
                #endregion
        
        #endregion
        
        public SupervisedComputers()
        {
            NewSupervisedComputer = new SupervisedComputerCreateDto();
            EditingSupervisedComputer = new SupervisedComputerUpdateDto();
            Filter = new GetSupervisedComputersInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SupervisedComputerList = new List<SupervisedComputerDto>();
            
            NewSnitchingLog = new SnitchingLogCreateDto();
EditingSnitchingLog = new SnitchingLogUpdateDto();
NewActivityRecord = new ActivityRecordCreateDto();
EditingActivityRecord = new ActivityRecordUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SupervisedComputers"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            
            
            Toolbar.AddButton(L["NewSupervisedComputer"], async () =>
            {
                await OpenCreateSupervisedComputerModalAsync();
            }, IconName.Add, requiredPolicyName: SnitcherPortalPermissions.SupervisedComputers.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSupervisedComputer = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.SupervisedComputers.Create);
            CanEditSupervisedComputer = await AuthorizationService
                            .IsGrantedAsync(SnitcherPortalPermissions.SupervisedComputers.Edit);
            CanDeleteSupervisedComputer = await AuthorizationService
                            .IsGrantedAsync(SnitcherPortalPermissions.SupervisedComputers.Delete);
                            
            
            #region SnitchingLogs
            CanListSnitchingLog = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.SnitchingLogs.Default);
            CanCreateSnitchingLog = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.SnitchingLogs.Create);
            CanEditSnitchingLog = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.SnitchingLogs.Edit);
            CanDeleteSnitchingLog = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.SnitchingLogs.Delete);
            #endregion

            #region ActivityRecords
            CanListActivityRecord = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.ActivityRecords.Default);
            CanCreateActivityRecord = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.ActivityRecords.Create);
            CanEditActivityRecord = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.ActivityRecords.Edit);
            CanDeleteActivityRecord = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.ActivityRecords.Delete);
            #endregion                
        }

        private async Task GetSupervisedComputersAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SupervisedComputersAppService.GetListAsync(Filter);
            SupervisedComputerList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSupervisedComputersAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SupervisedComputerDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSupervisedComputersAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSupervisedComputerModalAsync()
        {
            NewSupervisedComputer = new SupervisedComputerCreateDto{
                BanUntil = DateTime.Now,

                
            };
            await NewSupervisedComputerValidations.ClearAll();
            await CreateSupervisedComputerModal.Show();
        }

        private async Task CloseCreateSupervisedComputerModalAsync()
        {
            NewSupervisedComputer = new SupervisedComputerCreateDto{
                BanUntil = DateTime.Now,

                
            };
            await CreateSupervisedComputerModal.Hide();
        }

        private async Task OpenEditSupervisedComputerModalAsync(SupervisedComputerDto input)
        {
            var supervisedComputer = await SupervisedComputersAppService.GetAsync(input.Id);
            
            EditingSupervisedComputerId = supervisedComputer.Id;
            EditingSupervisedComputer = ObjectMapper.Map<SupervisedComputerDto, SupervisedComputerUpdateDto>(supervisedComputer);
            await EditingSupervisedComputerValidations.ClearAll();
            await EditSupervisedComputerModal.Show();
        }

        private async Task DeleteSupervisedComputerAsync(SupervisedComputerDto input)
        {
            await SupervisedComputersAppService.DeleteAsync(input.Id);
            await GetSupervisedComputersAsync();
        }

        private async Task CreateSupervisedComputerAsync()
        {
            try
            {
                if (await NewSupervisedComputerValidations.ValidateAll() == false)
                {
                    return;
                }

                await SupervisedComputersAppService.CreateAsync(NewSupervisedComputer);
                await GetSupervisedComputersAsync();
                await CloseCreateSupervisedComputerModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSupervisedComputerModalAsync()
        {
            await EditSupervisedComputerModal.Hide();
        }

        private async Task UpdateSupervisedComputerAsync()
        {
            try
            {
                if (await EditingSupervisedComputerValidations.ValidateAll() == false)
                {
                    return;
                }

                await SupervisedComputersAppService.UpdateAsync(EditingSupervisedComputerId, EditingSupervisedComputer);
                await GetSupervisedComputersAsync();
                await EditSupervisedComputerModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }

        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnIdentifierChangedAsync(string? identifier)
        {
            Filter.Identifier = identifier;
            await SearchAsync();
        }
        protected virtual async Task OnIpAddressChangedAsync(string? ipAddress)
        {
            Filter.IpAddress = ipAddress;
            await SearchAsync();
        }
        protected virtual async Task OnCalendarChangedAsync(string? calendar)
        {
            Filter.Calendar = calendar;
            await SearchAsync();
        }
        protected virtual async Task OnIsCalendarActiveChangedAsync(bool? isCalendarActive)
        {
            Filter.IsCalendarActive = isCalendarActive;
            await SearchAsync();
        }
        protected virtual async Task OnBanUntilMinChangedAsync(DateTime? banUntilMin)
        {
            Filter.BanUntilMin = banUntilMin.HasValue ? banUntilMin.Value.Date : banUntilMin;
            await SearchAsync();
        }
        protected virtual async Task OnBanUntilMaxChangedAsync(DateTime? banUntilMax)
        {
            Filter.BanUntilMax = banUntilMax.HasValue ? banUntilMax.Value.Date.AddDays(1).AddSeconds(-1) : banUntilMax;
            await SearchAsync();
        }
        


    private bool ShouldShowDetailRow()
    {
        return CanListSnitchingLog ||CanListActivityRecord;
    }
    
    public string SelectedChildTab { get; set; } = "snitchinglog-tab";
        
    private Task OnSelectedChildTabChanged(string name)
    {
        SelectedChildTab = name;
    
        return Task.CompletedTask;
    }


        #region SnitchingLogs
        
        private async Task OnSnitchingLogDataGridReadAsync(DataGridReadDataEventArgs<SnitchingLogDto> e, Guid supervisedComputerId)
        {
            var sorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            var currentPage = e.Page;
            await SetSnitchingLogsAsync(supervisedComputerId, currentPage, sorting: sorting);
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task SetSnitchingLogsAsync(Guid supervisedComputerId, int currentPage = 1, string? sorting = null)
        {
            var supervisedComputer = SupervisedComputerList.FirstOrDefault(x => x.Id == supervisedComputerId);
            if(supervisedComputer == null)
            {
                return;
            }

            var snitchingLogs = await SnitchingLogsAppService.GetListBySupervisedComputerIdAsync(new GetSnitchingLogListInput 
            {
                SupervisedComputerId = supervisedComputerId,
                MaxResultCount = SnitchingLogPageSize,
                SkipCount = (currentPage - 1) * SnitchingLogPageSize,
                Sorting = sorting
            });

            supervisedComputer.SnitchingLogs = snitchingLogs.Items.ToList();

            var snitchingLogDataGrid = SnitchingLogDataGrids[supervisedComputerId];
            
            snitchingLogDataGrid.CurrentPage = currentPage;
            snitchingLogDataGrid.TotalItems = (int)snitchingLogs.TotalCount;
        }
        
        private async Task OpenEditSnitchingLogModalAsync(SnitchingLogDto input)
        {
            var snitchingLog = await SnitchingLogsAppService.GetAsync(input.Id);

            EditingSnitchingLogId = snitchingLog.Id;
            EditingSnitchingLog = ObjectMapper.Map<SnitchingLogDto, SnitchingLogUpdateDto>(snitchingLog);
            await EditingSnitchingLogValidations.ClearAll();
            await EditSnitchingLogModal.Show();
        }
        
        private async Task CloseEditSnitchingLogModalAsync()
        {
            await EditSnitchingLogModal.Hide();
        }
        
        private async Task UpdateSnitchingLogAsync()
        {
            try
            {
                if (await EditingSnitchingLogValidations.ValidateAll() == false)
                {
                    return;
                }

                await SnitchingLogsAppService.UpdateAsync(EditingSnitchingLogId, EditingSnitchingLog);
                await SetSnitchingLogsAsync(EditingSnitchingLog.SupervisedComputerId);
                await EditSnitchingLogModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async Task DeleteSnitchingLogAsync(SnitchingLogDto input)
        {
            await SnitchingLogsAppService.DeleteAsync(input.Id);
            await SetSnitchingLogsAsync(input.SupervisedComputerId);
        }
        
        private async Task OpenCreateSnitchingLogModalAsync(Guid supervisedComputerId)
        {
            NewSnitchingLog = new SnitchingLogCreateDto
            {
                SupervisedComputerId = supervisedComputerId
            };

            await NewSnitchingLogValidations.ClearAll();
            await CreateSnitchingLogModal.Show();
        }
        
        private async Task CloseCreateSnitchingLogModalAsync()
        {
            NewSnitchingLog = new SnitchingLogCreateDto();

            await CreateSnitchingLogModal.Hide();
        }
        
        private async Task CreateSnitchingLogAsync()
        {
            try
            {
                if (await NewSnitchingLogValidations.ValidateAll() == false)
                {
                    return;
                }

                await SnitchingLogsAppService.CreateAsync(NewSnitchingLog);
                await SetSnitchingLogsAsync(NewSnitchingLog.SupervisedComputerId);
                await CloseCreateSnitchingLogModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        
        
        #endregion

        #region ActivityRecords
        
        private async Task OnActivityRecordDataGridReadAsync(DataGridReadDataEventArgs<ActivityRecordDto> e, Guid supervisedComputerId)
        {
            var sorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            var currentPage = e.Page;
            await SetActivityRecordsAsync(supervisedComputerId, currentPage, sorting: sorting);
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task SetActivityRecordsAsync(Guid supervisedComputerId, int currentPage = 1, string? sorting = null)
        {
            var supervisedComputer = SupervisedComputerList.FirstOrDefault(x => x.Id == supervisedComputerId);
            if(supervisedComputer == null)
            {
                return;
            }

            var activityRecords = await ActivityRecordsAppService.GetListBySupervisedComputerIdAsync(new GetActivityRecordListInput 
            {
                SupervisedComputerId = supervisedComputerId,
                MaxResultCount = ActivityRecordPageSize,
                SkipCount = (currentPage - 1) * ActivityRecordPageSize,
                Sorting = sorting
            });

            supervisedComputer.ActivityRecords = activityRecords.Items.ToList();

            var activityRecordDataGrid = ActivityRecordDataGrids[supervisedComputerId];
            
            activityRecordDataGrid.CurrentPage = currentPage;
            activityRecordDataGrid.TotalItems = (int)activityRecords.TotalCount;
        }
        
        private async Task OpenEditActivityRecordModalAsync(ActivityRecordDto input)
        {
            var activityRecord = await ActivityRecordsAppService.GetAsync(input.Id);

            EditingActivityRecordId = activityRecord.Id;
            EditingActivityRecord = ObjectMapper.Map<ActivityRecordDto, ActivityRecordUpdateDto>(activityRecord);
            await EditingActivityRecordValidations.ClearAll();
            await EditActivityRecordModal.Show();
        }
        
        private async Task CloseEditActivityRecordModalAsync()
        {
            await EditActivityRecordModal.Hide();
        }
        
        private async Task UpdateActivityRecordAsync()
        {
            try
            {
                if (await EditingActivityRecordValidations.ValidateAll() == false)
                {
                    return;
                }

                await ActivityRecordsAppService.UpdateAsync(EditingActivityRecordId, EditingActivityRecord);
                await SetActivityRecordsAsync(EditingActivityRecord.SupervisedComputerId);
                await EditActivityRecordModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async Task DeleteActivityRecordAsync(ActivityRecordDto input)
        {
            await ActivityRecordsAppService.DeleteAsync(input.Id);
            await SetActivityRecordsAsync(input.SupervisedComputerId);
        }
        
        private async Task OpenCreateActivityRecordModalAsync(Guid supervisedComputerId)
        {
            NewActivityRecord = new ActivityRecordCreateDto
            {
                SupervisedComputerId = supervisedComputerId
            };

            await NewActivityRecordValidations.ClearAll();
            await CreateActivityRecordModal.Show();
        }
        
        private async Task CloseCreateActivityRecordModalAsync()
        {
            NewActivityRecord = new ActivityRecordCreateDto();

            await CreateActivityRecordModal.Hide();
        }
        
        private async Task CreateActivityRecordAsync()
        {
            try
            {
                if (await NewActivityRecordValidations.ValidateAll() == false)
                {
                    return;
                }

                await ActivityRecordsAppService.CreateAsync(NewActivityRecord);
                await SetActivityRecordsAsync(NewActivityRecord.SupervisedComputerId);
                await CloseCreateActivityRecordModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        
        
        #endregion
    }
}
