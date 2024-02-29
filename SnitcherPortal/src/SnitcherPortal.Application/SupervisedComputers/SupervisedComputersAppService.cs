using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using SnitcherPortal.Permissions;
using SnitcherPortal.Calendars;
using SnitcherPortal.Engine;
using SnitcherPortal.ActivityRecords;
using Volo.Abp.EventBus.Local;
using static EtoDefinitions;

namespace SnitcherPortal.SupervisedComputers;

[Authorize(SnitcherPortalPermissions.SupervisedComputers.Default)]
public class SupervisedComputersAppService : ApplicationService, ISupervisedComputersAppService
{

    protected ISupervisedComputerRepository _supervisedComputerRepository;
    protected IActivityRecordRepository _activityRecordRepository;
    protected SupervisedComputerManager _supervisedComputerManager;
    protected SnitcherClientFunctions _snitcherClientFunctions;
    protected CalendarManager _calendarManager;
    protected ILocalEventBus _localEventBus;

    public SupervisedComputersAppService(ISupervisedComputerRepository supervisedComputerRepository,
        IActivityRecordRepository activityRecordRepository,
        SupervisedComputerManager supervisedComputerManager,
        SnitcherClientFunctions snitcherClientFunctions,
        CalendarManager calendarManager,
        ILocalEventBus localEventBus)
    {
        _calendarManager = calendarManager;
        _supervisedComputerRepository = supervisedComputerRepository;
        _activityRecordRepository = activityRecordRepository;
        _snitcherClientFunctions = snitcherClientFunctions;
        _supervisedComputerManager = supervisedComputerManager;
        _localEventBus = localEventBus;
    }

    public virtual async Task<PagedResultDto<SupervisedComputerDto>> GetListAsync(GetSupervisedComputersInput input)
    {
        var totalCount = await _supervisedComputerRepository.GetCountAsync(input.FilterText, input.Name, input.Identifier, input.ConnectionId, input.IsCalendarActive, input.BanUntilMin, input.BanUntilMax);
        var items = await _supervisedComputerRepository.GetListAsync(input.FilterText, input.Name, input.Identifier, input.ConnectionId, input.IsCalendarActive, input.BanUntilMin, input.BanUntilMax, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<SupervisedComputerDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<SupervisedComputer>, List<SupervisedComputerDto>>(items)
        };
    }

    public virtual async Task<SupervisedComputerDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(await _supervisedComputerRepository.GetAsync(id));
    }

    [AllowAnonymous]
    public virtual async Task<SupervisedComputerDto> GetAsync(string connectionId)
    {
        var data = (await _supervisedComputerRepository.GetQueryableNoTrackingAsync())
            .Where(e => e.ConnectionId == connectionId).FirstOrDefault();
        return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(data!);
    }

    [Authorize(SnitcherPortalPermissions.SupervisedComputers.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _supervisedComputerRepository.DeleteAsync(id);
    }

    [Authorize(SnitcherPortalPermissions.SupervisedComputers.Create)]
    public virtual async Task<SupervisedComputerDto> CreateAsync(SupervisedComputerCreateDto input)
    {
        var supervisedComputer = await _supervisedComputerManager.CreateAsync(input.Name, input.Identifier, input.IsCalendarActive,
            input.ClientHeartbeat, input.EnableAutokillReasoning, input.ConnectionId, input.BanUntil);
        return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(supervisedComputer);
    }

    [Authorize(SnitcherPortalPermissions.SupervisedComputers.Edit)]
    public virtual async Task<SupervisedComputerDto> UpdateAsync(Guid id, SupervisedComputerUpdateDto input)
    {
        var supervisedComputer = await _supervisedComputerManager.UpdateAsync(id,
        input.Name, input.Identifier, input.IsCalendarActive, input.Status, input.ClientHeartbeat, input.EnableAutokillReasoning,
        input.ConnectionId, input.BanUntil, input.ConcurrencyStamp);

        return ObjectMapper.Map<SupervisedComputer, SupervisedComputerDto>(supervisedComputer);
    }

    public async Task<List<string>> GetAvailableComputersAsync()
    {
        return (await _supervisedComputerRepository.GetQueryableNoTrackingAsync())
            .Select(e => e.Name).ToList();
    }

    public async Task<DashboardDataDto> GetDashboardDataAsync(string computerName)
    {
        var supervisedComputer = (await _supervisedComputerRepository.GetQueryableNoTrackingAsync(true)).First(sc => sc.Name == computerName);
        var activityRecords = (await _activityRecordRepository.GetQueryableNoTrackingAsync(false))
            .OrderByDescending(e => e.StartTime).Take(10).ToList();
        return DashboardMapper.Map(supervisedComputer, activityRecords, null);
    }

    public async Task KillProcessAsync(string computerName, string processName)
    {
        var supervisedComputer = (await _supervisedComputerRepository.GetQueryableNoTrackingAsync(false)).First(sc => sc.Name == computerName);

        await _localEventBus.PublishAsync(new KillCommandEto() { ConnectionId = supervisedComputer.ConnectionId, Processes = [processName] }, false);

        var updatedActivity = (await _activityRecordRepository.GetQueryableAsync()).FirstOrDefault(e => e.EndTime == null);
        if (updatedActivity != null)
        {
            updatedActivity.Data += updatedActivity.Data.IsNullOrWhiteSpace() == false ? "; " : "";
            updatedActivity.Data += $"'{processName}' killed manually at {DateTime.Now:HH:mm:ss}";
            await _activityRecordRepository.UpdateAsync(updatedActivity);
        }
    }

    public async Task SendMessageAsync(string computerName, string message)
    {
        var supervisedComputer = (await _supervisedComputerRepository.GetQueryableNoTrackingAsync(false)).First(sc => sc.Name == computerName);
        await _localEventBus.PublishAsync(new ShowMessageEto()
        {
            ConnectionId = supervisedComputer.ConnectionId,
            Duration = 4,
            Message = message
        }, false);
    }
}