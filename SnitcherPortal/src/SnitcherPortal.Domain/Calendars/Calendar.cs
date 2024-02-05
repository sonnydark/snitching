using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace SnitcherPortal.Calendars
{
    public class Calendar : Entity<Guid>
    {
        public virtual Guid SupervisedComputerId { get; set; }

        public virtual int DayOfWeek { get; set; }

        [CanBeNull]
        public virtual string? AllowedHours { get; set; }

        protected Calendar()
        {

        }

        public Calendar(Guid id, Guid supervisedComputerId, int dayOfWeek, string? allowedHours = null)
        {

            Id = id;
            SupervisedComputerId = supervisedComputerId;
            DayOfWeek = dayOfWeek;
            AllowedHours = allowedHours;
        }

    }
}