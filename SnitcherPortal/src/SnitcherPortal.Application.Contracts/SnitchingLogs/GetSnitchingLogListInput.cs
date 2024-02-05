using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.SnitchingLogs
{
    public class GetSnitchingLogListInput : PagedAndSortedResultRequestDto
    {
        public Guid SupervisedComputerId { get; set; }
    }
}