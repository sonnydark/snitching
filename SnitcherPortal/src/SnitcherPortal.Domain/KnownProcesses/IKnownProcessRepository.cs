using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SnitcherPortal.KnownProcesses
{
    public interface IKnownProcessRepository : IRepository<KnownProcess, Guid>
    {
        Task<List<KnownProcess>> GetListBySupervisedComputerIdAsync(
    Guid supervisedComputerId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default);

        Task<List<KnownProcess>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    bool? isHidden = null,
                    bool? isImportant = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            bool? isHidden = null,
            bool? isImportant = null,
            CancellationToken cancellationToken = default);
    }
}