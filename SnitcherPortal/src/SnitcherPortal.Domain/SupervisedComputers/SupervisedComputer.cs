using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.ActivityRecords;

using Volo.Abp;

namespace SnitcherPortal.SupervisedComputers
{
    public class SupervisedComputer : AggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string Identifier { get; set; }

        [CanBeNull]
        public virtual string? IpAddress { get; set; }

        [CanBeNull]
        public virtual string? Calendar { get; set; }

        public virtual bool IsCalendarActive { get; set; }

        public virtual DateTime? BanUntil { get; set; }

        public ICollection<SnitchingLog> SnitchingLogs { get; private set; }
        public ICollection<ActivityRecord> ActivityRecords { get; private set; }

        protected SupervisedComputer()
        {

        }

        public SupervisedComputer(Guid id, string name, string identifier, bool isCalendarActive, string? ipAddress = null, string? calendar = null, DateTime? banUntil = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), SupervisedComputerConsts.NameMaxLength, SupervisedComputerConsts.NameMinLength);
            Check.NotNull(identifier, nameof(identifier));
            Check.Length(identifier, nameof(identifier), SupervisedComputerConsts.IdentifierMaxLength, SupervisedComputerConsts.IdentifierMinLength);
            Check.Length(ipAddress, nameof(ipAddress), SupervisedComputerConsts.IpAddressMaxLength, 0);
            Name = name;
            Identifier = identifier;
            IsCalendarActive = isCalendarActive;
            IpAddress = ipAddress;
            Calendar = calendar;
            BanUntil = banUntil;
            SnitchingLogs = new Collection<SnitchingLog>();
            ActivityRecords = new Collection<ActivityRecord>();
        }

    }
}