using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SnitcherPortal.SupervisedComputers
{
    public interface ISupervisedComputerRepository : IRepository<SupervisedComputer, Guid>
    {
        Task<List<SupervisedComputer>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? identifier = null,
            string? connectionId = null,
            bool? isCalendarActive = null,
            DateTime? banUntilMin = null,
            DateTime? banUntilMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? identifier = null,
            string? connectionId = null,
            bool? isCalendarActive = null,
            DateTime? banUntilMin = null,
            DateTime? banUntilMax = null,
            CancellationToken cancellationToken = default);

        Task<IQueryable<SupervisedComputer>> GetQueryableNoTrackingAsync(bool includeDetails = false);
    }
}