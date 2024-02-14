using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnitcherPortal.ActivityRecords
{
    public class ActivityRecordCreateDto
    {
        public Guid SupervisedComputerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Data { get; set; }
    }
}