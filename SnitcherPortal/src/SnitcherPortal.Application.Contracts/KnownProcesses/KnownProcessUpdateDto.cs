using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnitcherPortal.KnownProcesses
{
    public class KnownProcessUpdateDto
    {
        public Guid SupervisedComputerId { get; set; }
        [Required]
        [StringLength(KnownProcessConsts.NameMaxLength, MinimumLength = KnownProcessConsts.NameMinLength)]
        public string Name { get; set; } = null!;
        public bool IsHidden { get; set; }
        public bool IsImportant { get; set; }

    }
}