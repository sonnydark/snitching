using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SnitcherPortal.SnitchingLogs
{
    public interface ISnitchingLogRepository : IRepository<SnitchingLog, Guid>
    {
        Task<List<SnitchingLog>> GetListBySupervisedComputerIdAsync(
    Guid supervisedComputerId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default);

        Task<List<SnitchingLog>> GetListAsync(
                    string? filterText = null,
                    DateTime? timestampMin = null,
                    DateTime? timestampMax = null,
                    string? message = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            DateTime? timestampMin = null,
            DateTime? timestampMax = null,
            string? message = null,
            CancellationToken cancellationToken = default);
    }
}