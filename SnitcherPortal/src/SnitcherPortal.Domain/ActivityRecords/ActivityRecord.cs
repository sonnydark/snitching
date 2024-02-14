using System;
using Volo.Abp.Domain.Entities;
using JetBrains.Annotations;

namespace SnitcherPortal.ActivityRecords
{
    public class ActivityRecord : Entity<Guid>
    {
        public virtual Guid SupervisedComputerId { get; set; }

        public virtual DateTime StartTime { get; set; }
        public virtual DateTime LastUpdateTime { get; set; }

        public virtual DateTime? EndTime { get; set; }

        [CanBeNull]
        public virtual string? Data { get; set; }

        protected ActivityRecord()
        {

        }

        public ActivityRecord(Guid id, Guid supervisedComputerId, DateTime startTime, DateTime? endTime = null, string? data = null)
        {

            Id = id;
            SupervisedComputerId = supervisedComputerId;
            StartTime = startTime;
            LastUpdateTime = startTime;
            EndTime = endTime;
            Data = data;
        }
    }
}