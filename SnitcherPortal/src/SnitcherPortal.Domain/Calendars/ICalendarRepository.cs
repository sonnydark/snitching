using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SnitcherPortal.Calendars
{
    public interface ICalendarRepository : IRepository<Calendar, Guid>
    {
        Task<List<Calendar>> GetListBySupervisedComputerIdAsync(
    Guid supervisedComputerId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default);

        Task<List<Calendar>> GetListAsync(
                    string? filterText = null,
                    int? dayOfWeekMin = null,
                    int? dayOfWeekMax = null,
                    string? allowedHours = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? dayOfWeekMin = null,
            int? dayOfWeekMax = null,
            string? allowedHours = null,
            CancellationToken cancellationToken = default);
    }
}