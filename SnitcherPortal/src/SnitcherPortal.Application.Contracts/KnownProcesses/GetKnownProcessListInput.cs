using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.KnownProcesses
{
    public class GetKnownProcessListInput : PagedAndSortedResultRequestDto
    {
        public Guid SupervisedComputerId { get; set; }
    }
}