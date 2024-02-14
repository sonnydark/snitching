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
using SnitcherPortal.SupervisedComputers;

namespace SnitcherPortal.ActivityRecords
{
    public class EfCoreActivityRecordRepository : EfCoreRepository<SnitcherPortalDbContext, ActivityRecord, Guid>, IActivityRecordRepository
    {
        public EfCoreActivityRecordRepository(IDbContextProvider<SnitcherPortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<ActivityRecord>> GetListBySupervisedComputerIdAsync(
           Guid supervisedComputerId,
           string? sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ActivityRecordConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId).CountAsync(cancellationToken);
        }

        public virtual async Task<List<ActivityRecord>> GetListAsync(
            string? filterText = null,
            DateTime? startTimeMin = null,
            DateTime? startTimeMax = null,
            DateTime? endTimeMin = null,
            DateTime? endTimeMax = null,
            string? data = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, startTimeMin, startTimeMax, endTimeMin, endTimeMax, data);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ActivityRecordConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            DateTime? startTimeMin = null,
            DateTime? startTimeMax = null,
            DateTime? endTimeMin = null,
            DateTime? endTimeMax = null,
            string? data = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, startTimeMin, startTimeMax, endTimeMin, endTimeMax, data);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ActivityRecord> ApplyFilter(
            IQueryable<ActivityRecord> query,
            string? filterText = null,
            DateTime? startTimeMin = null,
            DateTime? startTimeMax = null,
            DateTime? endTimeMin = null,
            DateTime? endTimeMax = null,
            string? data = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Data!.Contains(filterText!))
                    .WhereIf(startTimeMin.HasValue, e => e.StartTime >= startTimeMin!.Value)
                    .WhereIf(startTimeMax.HasValue, e => e.StartTime <= startTimeMax!.Value)
                    .WhereIf(endTimeMin.HasValue, e => e.EndTime >= endTimeMin!.Value)
                    .WhereIf(endTimeMax.HasValue, e => e.EndTime <= endTimeMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(data), e => e.Data.Contains(data));
        }

        public virtual async Task<IQueryable<ActivityRecord>> GetQueryableNoTrackingAsync(bool includeDetails = false)
        {
            var query = includeDetails ? await WithDetailsAsync() : await this.GetQueryableAsync();
            return query.AsNoTracking();
        }

        public override async Task<IQueryable<ActivityRecord>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).IncludeDetails();
        }
    }

    public static class IncludeDetailsExtension
    {
        public static IQueryable<ActivityRecord> IncludeDetails(this IQueryable<ActivityRecord> queryable)
        {
            return queryable;
        }
    }
}