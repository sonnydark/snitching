using SnitcherPortal.KnownProcesses;
using SnitcherPortal.Calendars;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.ActivityRecords;
using System;
using SnitcherPortal.Shared;
using Volo.Abp.AutoMapper;
using SnitcherPortal.SupervisedComputers;
using AutoMapper;

namespace SnitcherPortal;

public class SnitcherPortalApplicationAutoMapperProfile : Profile
{
    public SnitcherPortalApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<SupervisedComputer, SupervisedComputerDto>().Ignore(x => x.ActivityRecords);

        CreateMap<ActivityRecord, ActivityRecordDto>();

        CreateMap<SnitchingLog, SnitchingLogDto>();

        CreateMap<SupervisedComputer, SupervisedComputerDto>().Ignore(x => x.SnitchingLogs).Ignore(x => x.ActivityRecords);

        CreateMap<Calendar, CalendarDto>();

        CreateMap<KnownProcess, KnownProcessDto>();
    }
}