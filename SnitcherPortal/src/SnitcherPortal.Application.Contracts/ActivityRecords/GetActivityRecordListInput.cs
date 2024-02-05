using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.ActivityRecords
{
    public class GetActivityRecordListInput : PagedAndSortedResultRequestDto
    {
        public Guid SupervisedComputerId { get; set; }
    }
}