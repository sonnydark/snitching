using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.ActivityRecords;
using SnitcherPortal.Calendars;
using SnitcherPortal.KnownProcesses;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace SnitcherPortal.SupervisedComputers;

public class SupervisedComputer : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string Name { get; set; }

    [NotNull]
    public virtual string Identifier { get; set; }

    [CanBeNull]
    public virtual string? ConnectionId { get; set; }

    public virtual SupervisedComputerStatus Status { get; set; }

    public virtual bool IsCalendarActive { get; set; }

    public virtual DateTime? BanUntil { get; set; }

    public virtual int ClientHeartbeat { get; set; }

    public virtual bool EnableAutokillReasoning { get; set; }

    public ICollection<SnitchingLog> SnitchingLogs { get; private set; }
    public ICollection<ActivityRecord> ActivityRecords { get; private set; }
    public ICollection<Calendar> Calendars { get; private set; }
    public ICollection<KnownProcess> KnownProcesses { get; private set; }

    protected SupervisedComputer()
    {

    }

    public SupervisedComputer(Guid id, string name, string identifier, bool isCalendarActive, string? ipAddress = null, DateTime? banUntil = null)
    {

        Id = id;
        Check.NotNull(name, nameof(name));
        Check.Length(name, nameof(name), SupervisedComputerConsts.NameMaxLength, SupervisedComputerConsts.NameMinLength);
        Check.NotNull(identifier, nameof(identifier));
        Check.Length(identifier, nameof(identifier), SupervisedComputerConsts.IdentifierMaxLength, SupervisedComputerConsts.IdentifierMinLength);
        Check.Length(ipAddress, nameof(ipAddress), SupervisedComputerConsts.ConnectionIdMaxLength, 0);
        Name = name;
        Identifier = identifier;
        IsCalendarActive = isCalendarActive;
        ConnectionId = ipAddress;
        BanUntil = banUntil;
        SnitchingLogs = new Collection<SnitchingLog>();
        ActivityRecords = new Collection<ActivityRecord>();
        Calendars = new Collection<Calendar>();
        KnownProcesses = new Collection<KnownProcess>();
    }

}