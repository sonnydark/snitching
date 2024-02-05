using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace SnitcherPortal.KnownProcesses
{
    public class KnownProcessDto : EntityDto<Guid>
    {
        public Guid SupervisedComputerId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsHidden { get; set; }
        public bool IsImportant { get; set; }

    }
}