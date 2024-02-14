using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace SnitcherPortal.ActivityRecords
{
    public class ActivityRecordDto : EntityDto<Guid>
    {
        public Guid SupervisedComputerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Data { get; set; }

    }
}