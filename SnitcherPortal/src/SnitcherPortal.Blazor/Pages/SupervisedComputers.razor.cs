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
using SnitcherPortal.Calendars;
using SnitcherPortal.KnownProcesses;


namespace SnitcherPortal.Blazor.Pages
{
    public partial class SupervisedComputers
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
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
        private SnitchingLogCreateDto NewSnitchingLog { get; set; }
        private Dictionary<Guid, DataGrid<SnitchingLogDto>> SnitchingLogDataGrids { get; set; } = new();
        private int SnitchingLogPageSize { get; } = 5;
        private Validations NewSnitchingLogValidations { get; set; } = new();
        private Modal CreateSnitchingLogModal { get; set; } = new();
        private Guid EditingSnitchingLogId { get; set; }
        private SnitchingLogUpdateDto EditingSnitchingLog { get; set; }
        private Validations EditingSnitchingLogValidations { get; set; } = new();
        private Modal EditSnitchingLogModal { get; set; } = new();


        #endregion
        #region ActivityRecords

        private bool CanListActivityRecord { get; set; }
        private ActivityRecordCreateDto NewActivityRecord { get; set; }
        private Dictionary<Guid, DataGrid<ActivityRecordDto>> ActivityRecordDataGrids { get; set; } = new();
        private int ActivityRecordPageSize { get; } = 5;
        private Validations NewActivityRecordValidations { get; set; } = new();
        private Modal CreateActivityRecordModal { get; set; } = new();
        private Guid EditingActivityRecordId { get; set; }
        private ActivityRecordUpdateDto EditingActivityRecord { get; set; }
        private Validations EditingActivityRecordValidations { get; set; } = new();
        private Modal EditActivityRecordModal { get; set; } = new();

        #endregion
        #region Calendars

        private bool CanListCalendar { get; set; }
        private bool CanCreateCalendar { get; set; }
        private bool CanEditCalendar { get; set; }
        private bool CanDeleteCalendar { get; set; }
        private CalendarCreateDto NewCalendar { get; set; }
        private Dictionary<Guid, DataGrid<CalendarDto>> CalendarDataGrids { get; set; } = new();
        private int CalendarPageSize { get; } = 7;
        private DataGridEntityActionsColumn<CalendarDto> CalendarEntityActionsColumns { get; set; } = new();
        private Validations NewCalendarValidations { get; set; } = new();
        private Modal CreateCalendarModal { get; set; } = new();
        private Guid EditingCalendarId { get; set; }
        private CalendarUpdateDto EditingCalendar { get; set; }
        private Validations EditingCalendarValidations { get; set; } = new();
        private Modal EditCalendarModal { get; set; } = new();


        #endregion
        #region KnownProcesses

        private bool CanListKnownProcess { get; set; }
        private bool CanCreateKnownProcess { get; set; }
        private bool CanEditKnownProcess { get; set; }
        private bool CanDeleteKnownProcess { get; set; }
        private KnownProcessCreateDto NewKnownProcess { get; set; }
        private Dictionary<Guid, DataGrid<KnownProcessDto>> KnownProcessDataGrids { get; set; } = new();
        private int KnownProcessPageSize { get; } = int.MaxValue;
        private DataGridEntityActionsColumn<KnownProcessDto> KnownProcessEntityActionsColumns { get; set; } = new();
        private Validations NewKnownProcessValidations { get; set; } = new();
        private Modal CreateKnownProcessModal { get; set; } = new();
        private Guid EditingKnownProcessId { get; set; }
        private KnownProcessUpdateDto EditingKnownProcess { get; set; }
        private Validations EditingKnownProcessValidations { get; set; } = new();
        private Modal EditKnownProcessModal { get; set; } = new();


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
            NewCalendar = new CalendarCreateDto();
            EditingCalendar = new CalendarUpdateDto();
            NewKnownProcess = new KnownProcessCreateDto();
            EditingKnownProcess = new KnownProcessUpdateDto();
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
            
            #endregion

            #region ActivityRecords
            CanListActivityRecord = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.ActivityRecords.Default);
            
            #endregion

            #region Calendars
            CanListCalendar = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.Calendars.Default);
            CanCreateCalendar = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.Calendars.Create);
            CanEditCalendar = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.Calendars.Edit);
            CanDeleteCalendar = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.Calendars.Delete);
            #endregion

            #region KnownProcesses
            CanListKnownProcess = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.KnownProcesses.Default);
            CanCreateKnownProcess = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.KnownProcesses.Create);
            CanEditKnownProcess = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.KnownProcesses.Edit);
            CanDeleteKnownProcess = await AuthorizationService
                .IsGrantedAsync(SnitcherPortalPermissions.KnownProcesses.Delete);
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
            NewSupervisedComputer = new SupervisedComputerCreateDto
            {
                BanUntil = DateTime.Now,
                ClientHeartbeat = 10,
            };
            await NewSupervisedComputerValidations.ClearAll();
            await CreateSupervisedComputerModal.Show();
        }

        private async Task CloseCreateSupervisedComputerModalAsync()
        {
            NewSupervisedComputer = new SupervisedComputerCreateDto
            {
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
        protected virtual async Task OnConnectionIdChangedAsync(string? connectionId)
        {
            Filter.ConnectionId = connectionId;
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
            return CanListSnitchingLog || CanListActivityRecord || CanListCalendar || CanListKnownProcess;
        }

        public string SelectedChildTab { get; set; } = "activityrecord-tab";

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
            if (supervisedComputer == null)
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
            if (supervisedComputer == null)
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

        #region Calendars

        private async Task OnCalendarDataGridReadAsync(DataGridReadDataEventArgs<CalendarDto> e, Guid supervisedComputerId)
        {
            var sorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            var currentPage = e.Page;
            await SetCalendarsAsync(supervisedComputerId, currentPage, sorting: sorting);
            await InvokeAsync(StateHasChanged);
        }

        private async Task SetCalendarsAsync(Guid supervisedComputerId, int currentPage = 1, string? sorting = null)
        {
            var supervisedComputer = SupervisedComputerList.FirstOrDefault(x => x.Id == supervisedComputerId);
            if (supervisedComputer == null)
            {
                return;
            }

            var calendars = await CalendarsAppService.GetListBySupervisedComputerIdAsync(new GetCalendarListInput
            {
                SupervisedComputerId = supervisedComputerId,
                MaxResultCount = CalendarPageSize,
                SkipCount = (currentPage - 1) * CalendarPageSize,
                Sorting = sorting
            });

            supervisedComputer.Calendars = calendars.Items.ToList();

            var calendarDataGrid = CalendarDataGrids[supervisedComputerId];

            calendarDataGrid.CurrentPage = currentPage;
            calendarDataGrid.TotalItems = (int)calendars.TotalCount;
        }

        private async Task OpenEditCalendarModalAsync(CalendarDto input)
        {   
            var calendar = await CalendarsAppService.GetAsync(input.Id);

            EditingCalendarId = calendar.Id;
            EditingCalendar = ObjectMapper.Map<CalendarDto, CalendarUpdateDto>(calendar);
            await EditingCalendarValidations.ClearAll();
            await EditCalendarModal.Show();
        }

        private async Task CloseEditCalendarModalAsync()
        {
            await EditCalendarModal.Hide();
        }

        private async Task UpdateCalendarAsync()
        {
            try
            {
                if (await EditingCalendarValidations.ValidateAll() == false)
                {
                    return;
                }

                await CalendarsAppService.UpdateAsync(EditingCalendarId, EditingCalendar);
                await SetCalendarsAsync(EditingCalendar.SupervisedComputerId);
                await EditCalendarModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task DeleteCalendarAsync(CalendarDto input)
        {
            await CalendarsAppService.DeleteAsync(input.Id);
            await SetCalendarsAsync(input.SupervisedComputerId);
        }

        private async Task OpenCreateCalendarModalAsync(Guid supervisedComputerId)
        {
            NewCalendar = new CalendarCreateDto
            {
                SupervisedComputerId = supervisedComputerId
            };

            await NewCalendarValidations.ClearAll();
            await CreateCalendarModal.Show();
        }

        private async Task CloseCreateCalendarModalAsync()
        {
            NewCalendar = new CalendarCreateDto();

            await CreateCalendarModal.Hide();
        }

        private async Task CreateCalendarAsync()
        {
            try
            {
                if (await NewCalendarValidations.ValidateAll() == false)
                {
                    return;
                }

                await CalendarsAppService.CreateAsync(NewCalendar);
                await SetCalendarsAsync(NewCalendar.SupervisedComputerId);
                await CloseCreateCalendarModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }



        #endregion

        #region KnownProcesses

        private async Task OnKnownProcessDataGridReadAsync(DataGridReadDataEventArgs<KnownProcessDto> e, Guid supervisedComputerId)
        {
            var sorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            var currentPage = e.Page;
            await SetKnownProcessesAsync(supervisedComputerId, currentPage, sorting: sorting);
            await InvokeAsync(StateHasChanged);
        }

        private async Task SetKnownProcessesAsync(Guid supervisedComputerId, int currentPage = 1, string? sorting = null)
        {
            var supervisedComputer = SupervisedComputerList.FirstOrDefault(x => x.Id == supervisedComputerId);
            if (supervisedComputer == null)
            {
                return;
            }

            var knownProcesses = await KnownProcessesAppService.GetListBySupervisedComputerIdAsync(new GetKnownProcessListInput
            {
                SupervisedComputerId = supervisedComputerId,
                MaxResultCount = KnownProcessPageSize,
                SkipCount = (currentPage - 1) * KnownProcessPageSize,
                Sorting = sorting
            });

            supervisedComputer.KnownProcesses = knownProcesses.Items
                .OrderByDescending(e => e.IsImportant).ThenBy(e => e.IsHidden).ThenBy(e => e.Name).ToList();

            var knownProcessDataGrid = KnownProcessDataGrids[supervisedComputerId];

            knownProcessDataGrid.CurrentPage = currentPage;
            knownProcessDataGrid.TotalItems = (int)knownProcesses.TotalCount;
        }

        private async Task OpenEditKnownProcessModalAsync(KnownProcessDto input)
        {
            var knownProcess = await KnownProcessesAppService.GetAsync(input.Id);

            EditingKnownProcessId = knownProcess.Id;
            EditingKnownProcess = ObjectMapper.Map<KnownProcessDto, KnownProcessUpdateDto>(knownProcess);
            await EditingKnownProcessValidations.ClearAll();
            await EditKnownProcessModal.Show();
        }

        private async Task CloseEditKnownProcessModalAsync()
        {
            await EditKnownProcessModal.Hide();
        }

        private async Task UpdateKnownProcessAsync()
        {
            try
            {
                if (await EditingKnownProcessValidations.ValidateAll() == false)
                {
                    return;
                }

                await KnownProcessesAppService.UpdateAsync(EditingKnownProcessId, EditingKnownProcess);
                await SetKnownProcessesAsync(EditingKnownProcess.SupervisedComputerId);
                await EditKnownProcessModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task DeleteKnownProcessAsync(KnownProcessDto input)
        {
            await KnownProcessesAppService.DeleteAsync(input.Id);
            await SetKnownProcessesAsync(input.SupervisedComputerId);
        }

        private async Task OpenCreateKnownProcessModalAsync(Guid supervisedComputerId)
        {
            NewKnownProcess = new KnownProcessCreateDto
            {
                SupervisedComputerId = supervisedComputerId
            };

            await NewKnownProcessValidations.ClearAll();
            await CreateKnownProcessModal.Show();
        }

        private async Task MarkUnmarkHiddenAsync(Guid scId, bool mark)
        {
            try
            {
                await this.KnownProcessesAppService.MarkUnmarkHiddenAsync(scId, mark);
                await SetKnownProcessesAsync(scId);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseCreateKnownProcessModalAsync()
        {
            NewKnownProcess = new KnownProcessCreateDto();

            await CreateKnownProcessModal.Hide();
        }

        private async Task CreateKnownProcessAsync()
        {
            try
            {
                if (await NewKnownProcessValidations.ValidateAll() == false)
                {
                    return;
                }

                await KnownProcessesAppService.CreateAsync(NewKnownProcess);
                await SetKnownProcessesAsync(NewKnownProcess.SupervisedComputerId);
                await CloseCreateKnownProcessModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }



        #endregion
    }
}
