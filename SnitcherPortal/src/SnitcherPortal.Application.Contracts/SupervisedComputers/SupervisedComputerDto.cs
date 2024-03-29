using System;
using System.Collections.Generic;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.ActivityRecords;
using SnitcherPortal.Calendars;
using SnitcherPortal.KnownProcesses;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace SnitcherPortal.SupervisedComputers
{
    public class SupervisedComputerDto : EntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public string Identifier { get; set; } = null!;
        public string? IpAddress { get; set; }
        public SupervisedComputerStatus Status { get; set; }
        public bool IsCalendarActive { get; set; }
        public DateTime? BanUntil { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        public List<SnitchingLogDto> SnitchingLogs { get; set; } = new();
        public List<ActivityRecordDto> ActivityRecords { get; set; } = new();
        public List<CalendarDto> Calendars { get; set; } = new();
        public List<KnownProcessDto> KnownProcesses { get; set; } = new();
    }
}