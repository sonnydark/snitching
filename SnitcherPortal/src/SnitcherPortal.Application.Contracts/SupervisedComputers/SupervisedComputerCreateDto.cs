using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnitcherPortal.SupervisedComputers
{
    public class SupervisedComputerCreateDto
    {
        [Required]
        [StringLength(SupervisedComputerConsts.NameMaxLength, MinimumLength = SupervisedComputerConsts.NameMinLength)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(SupervisedComputerConsts.IdentifierMaxLength, MinimumLength = SupervisedComputerConsts.IdentifierMinLength)]
        public string Identifier { get; set; } = null!;
        [StringLength(SupervisedComputerConsts.IpAddressMaxLength)]
        public string? IpAddress { get; set; }
        public string? Calendar { get; set; }
        public bool IsCalendarActive { get; set; }
        public DateTime? BanUntil { get; set; }
    }
}