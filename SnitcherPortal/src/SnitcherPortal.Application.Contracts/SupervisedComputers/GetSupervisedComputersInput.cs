using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.SupervisedComputers
{
    public class GetSupervisedComputersInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public string? Identifier { get; set; }
        public string? IpAddress { get; set; }
        public string? Calendar { get; set; }
        public bool? IsCalendarActive { get; set; }
        public DateTime? BanUntilMin { get; set; }
        public DateTime? BanUntilMax { get; set; }

        public GetSupervisedComputersInput()
        {

        }
    }
}