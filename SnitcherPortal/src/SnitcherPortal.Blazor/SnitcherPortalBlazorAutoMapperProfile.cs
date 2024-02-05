using SnitcherPortal.KnownProcesses;
using SnitcherPortal.Calendars;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.ActivityRecords;
using Volo.Abp.AutoMapper;
using SnitcherPortal.SupervisedComputers;
using AutoMapper;

namespace SnitcherPortal.Blazor;

public class SnitcherPortalBlazorAutoMapperProfile : Profile
{
    public SnitcherPortalBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<SupervisedComputerDto, SupervisedComputerUpdateDto>();

        CreateMap<ActivityRecordDto, ActivityRecordUpdateDto>();

        CreateMap<SnitchingLogDto, SnitchingLogUpdateDto>();

        CreateMap<CalendarDto, CalendarUpdateDto>();

        CreateMap<KnownProcessDto, KnownProcessUpdateDto>();
    }
}