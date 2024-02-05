using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using SnitcherPortal.EntityFrameworkCore;

namespace SnitcherPortal.SnitchingLogs
{
    public class EfCoreSnitchingLogRepository : EfCoreRepository<SnitcherPortalDbContext, SnitchingLog, Guid>, ISnitchingLogRepository
    {
        public EfCoreSnitchingLogRepository(IDbContextProvider<SnitcherPortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<SnitchingLog>> GetListBySupervisedComputerIdAsync(
           Guid supervisedComputerId,
           string? sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SnitchingLogConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId).CountAsync(cancellationToken);
        }

        public virtual async Task<List<SnitchingLog>> GetListAsync(
            string? filterText = null,
            DateTime? timestampMin = null,
            DateTime? timestampMax = null,
            string? message = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, timestampMin, timestampMax, message);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SnitchingLogConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            DateTime? timestampMin = null,
            DateTime? timestampMax = null,
            string? message = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, timestampMin, timestampMax, message);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SnitchingLog> ApplyFilter(
            IQueryable<SnitchingLog> query,
            string? filterText = null,
            DateTime? timestampMin = null,
            DateTime? timestampMax = null,
            string? message = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Message!.Contains(filterText!))
                    .WhereIf(timestampMin.HasValue, e => e.Timestamp >= timestampMin!.Value)
                    .WhereIf(timestampMax.HasValue, e => e.Timestamp <= timestampMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(message), e => e.Message.Contains(message));
        }
    }
}