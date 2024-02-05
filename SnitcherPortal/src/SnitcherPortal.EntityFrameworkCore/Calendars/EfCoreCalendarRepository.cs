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

namespace SnitcherPortal.Calendars
{
    public class EfCoreCalendarRepository : EfCoreRepository<SnitcherPortalDbContext, Calendar, Guid>, ICalendarRepository
    {
        public EfCoreCalendarRepository(IDbContextProvider<SnitcherPortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<Calendar>> GetListBySupervisedComputerIdAsync(
           Guid supervisedComputerId,
           string? sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CalendarConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId).CountAsync(cancellationToken);
        }

        public virtual async Task<List<Calendar>> GetListAsync(
            string? filterText = null,
            int? dayOfWeekMin = null,
            int? dayOfWeekMax = null,
            string? allowedHours = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, dayOfWeekMin, dayOfWeekMax, allowedHours);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CalendarConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? dayOfWeekMin = null,
            int? dayOfWeekMax = null,
            string? allowedHours = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, dayOfWeekMin, dayOfWeekMax, allowedHours);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Calendar> ApplyFilter(
            IQueryable<Calendar> query,
            string? filterText = null,
            int? dayOfWeekMin = null,
            int? dayOfWeekMax = null,
            string? allowedHours = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.AllowedHours!.Contains(filterText!))
                    .WhereIf(dayOfWeekMin.HasValue, e => e.DayOfWeek >= dayOfWeekMin!.Value)
                    .WhereIf(dayOfWeekMax.HasValue, e => e.DayOfWeek <= dayOfWeekMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(allowedHours), e => e.AllowedHours.Contains(allowedHours));
        }
    }
}