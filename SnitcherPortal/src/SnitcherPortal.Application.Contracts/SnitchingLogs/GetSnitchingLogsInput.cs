using Volo.Abp.Application.Dtos;
using System;

namespace SnitcherPortal.SnitchingLogs
{
    public class GetSnitchingLogsInput : PagedAndSortedResultRequestDto
    {
        public Guid? SupervisedComputerId { get; set; }

        public string? FilterText { get; set; }

        public DateTime? TimestampMin { get; set; }
        public DateTime? TimestampMax { get; set; }
        public string? Message { get; set; }

        public GetSnitchingLogsInput()
        {

        }
    }
}