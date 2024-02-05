using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace SnitcherPortal.ActivityRecords
{
    public class ActivityRecord : Entity<Guid>
    {
        public virtual Guid SupervisedComputerId { get; set; }

        public virtual DateTime StartTime { get; set; }

        public virtual DateTime? EndTime { get; set; }

        [CanBeNull]
        public virtual string? DetectedProcesses { get; set; }

        protected ActivityRecord()
        {

        }

        public ActivityRecord(Guid id, Guid supervisedComputerId, DateTime startTime, DateTime? endTime = null, string? detectedProcesses = null)
        {

            Id = id;
            SupervisedComputerId = supervisedComputerId;
            StartTime = startTime;
            EndTime = endTime;
            DetectedProcesses = detectedProcesses;
        }

    }
}