using System;
using System.ComponentModel.DataAnnotations;

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
        [StringLength(SupervisedComputerConsts.ConnectionIdMaxLength)]
        public string? ConnectionId { get; set; }
        public bool IsCalendarActive { get; set; }
        public DateTime? BanUntil { get; set; }
        public int ClientHeartbeat { get; set; }
        public bool EnableAutokillReasoning { get; set; }
    }
}