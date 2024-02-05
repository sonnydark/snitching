using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace SnitcherPortal.SnitchingLogs
{
    public class SnitchingLog : Entity<Guid>
    {
        public virtual Guid SupervisedComputerId { get; set; }

        public virtual DateTime Timestamp { get; set; }

        [CanBeNull]
        public virtual string? Message { get; set; }

        protected SnitchingLog()
        {

        }

        public SnitchingLog(Guid id, Guid supervisedComputerId, DateTime timestamp, string? message = null)
        {

            Id = id;
            SupervisedComputerId = supervisedComputerId;
            Timestamp = timestamp;
            Message = message;
        }

    }
}