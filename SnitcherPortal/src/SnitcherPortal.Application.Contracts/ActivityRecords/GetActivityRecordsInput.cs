using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.ActivityRecords
{
    public class GetActivityRecordsInput : PagedAndSortedResultRequestDto
    {
        public Guid? SupervisedComputerId { get; set; }

        public string? FilterText { get; set; }

        public DateTime? StartTimeMin { get; set; }
        public DateTime? StartTimeMax { get; set; }
        public DateTime? EndTimeMin { get; set; }
        public DateTime? EndTimeMax { get; set; }
        public string? Data { get; set; }

        public GetActivityRecordsInput()
        {

        }
    }
}