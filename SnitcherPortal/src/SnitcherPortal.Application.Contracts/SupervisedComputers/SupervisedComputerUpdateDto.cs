using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace SnitcherPortal.SupervisedComputers
{
    public class SupervisedComputerUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        [StringLength(SupervisedComputerConsts.NameMaxLength, MinimumLength = SupervisedComputerConsts.NameMinLength)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(SupervisedComputerConsts.IdentifierMaxLength, MinimumLength = SupervisedComputerConsts.IdentifierMinLength)]
        public string Identifier { get; set; } = null!;
        [StringLength(SupervisedComputerConsts.ConnectionIdMaxLength)]
        public string? ConnectionId { get; set; }
        public SupervisedComputerStatus Status { get; set; }
        public bool IsCalendarActive { get; set; }
        public DateTime? BanUntil { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}