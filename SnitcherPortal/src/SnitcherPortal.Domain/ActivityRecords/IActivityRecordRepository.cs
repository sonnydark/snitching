using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SnitcherPortal.ActivityRecords
{
    public interface IActivityRecordRepository : IRepository<ActivityRecord, Guid>
    {
        Task<List<ActivityRecord>> GetListBySupervisedComputerIdAsync(
    Guid supervisedComputerId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default);

        Task<List<ActivityRecord>> GetListAsync(
                    string? filterText = null,
                    DateTime? startTimeMin = null,
                    DateTime? startTimeMax = null,
                    DateTime? endTimeMin = null,
                    DateTime? endTimeMax = null,
                    string? data = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            DateTime? startTimeMin = null,
            DateTime? startTimeMax = null,
            DateTime? endTimeMin = null,
            DateTime? endTimeMax = null,
            string? data = null,
            CancellationToken cancellationToken = default);
    }
}