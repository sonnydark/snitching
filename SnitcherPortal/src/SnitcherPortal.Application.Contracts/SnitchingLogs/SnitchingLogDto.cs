using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace SnitcherPortal.SnitchingLogs
{
    public class SnitchingLogDto : EntityDto<Guid>
    {
        public Guid SupervisedComputerId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Message { get; set; }

    }
}