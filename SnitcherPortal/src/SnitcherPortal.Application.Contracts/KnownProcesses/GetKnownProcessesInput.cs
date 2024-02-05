using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.KnownProcesses
{
    public class GetKnownProcessesInput : PagedAndSortedResultRequestDto
    {
        public Guid? SupervisedComputerId { get; set; }

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsImportant { get; set; }

        public GetKnownProcessesInput()
        {

        }
    }
}