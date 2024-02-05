using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnitcherPortal.SnitchingLogs
{
    public class SnitchingLogUpdateDto
    {
        public Guid SupervisedComputerId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Message { get; set; }

    }
}