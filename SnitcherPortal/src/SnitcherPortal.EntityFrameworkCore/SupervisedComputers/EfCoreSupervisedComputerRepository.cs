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

namespace SnitcherPortal.SupervisedComputers
{
    public class EfCoreSupervisedComputerRepository : EfCoreRepository<SnitcherPortalDbContext, SupervisedComputer, Guid>, ISupervisedComputerRepository
    {
        public EfCoreSupervisedComputerRepository(IDbContextProvider<SnitcherPortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<SupervisedComputer>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? identifier = null,
            string? ipAddress = null,
            bool? isCalendarActive = null,
            DateTime? banUntilMin = null,
            DateTime? banUntilMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, identifier, ipAddress, isCalendarActive, banUntilMin, banUntilMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SupervisedComputerConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? identifier = null,
            string? ipAddress = null,
            bool? isCalendarActive = null,
            DateTime? banUntilMin = null,
            DateTime? banUntilMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, name, identifier, ipAddress, isCalendarActive, banUntilMin, banUntilMax);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SupervisedComputer> ApplyFilter(
            IQueryable<SupervisedComputer> query,
            string? filterText = null,
            string? name = null,
            string? identifier = null,
            string? ipAddress = null,
            bool? isCalendarActive = null,
            DateTime? banUntilMin = null,
            DateTime? banUntilMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!) || e.Identifier!.Contains(filterText!) || e.IpAddress!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(identifier), e => e.Identifier.Contains(identifier))
                    .WhereIf(!string.IsNullOrWhiteSpace(ipAddress), e => e.IpAddress.Contains(ipAddress))
                    .WhereIf(isCalendarActive.HasValue, e => e.IsCalendarActive == isCalendarActive)
                    .WhereIf(banUntilMin.HasValue, e => e.BanUntil >= banUntilMin!.Value)
                    .WhereIf(banUntilMax.HasValue, e => e.BanUntil <= banUntilMax!.Value);
        }

        public virtual async Task<IQueryable<SupervisedComputer>> GetQueryableNoTrackingAsync(bool includeDetails = false)
        {
            var query = includeDetails ? await WithDetailsAsync() : await this.GetQueryableAsync();
            return query.AsNoTracking();
        }

        public override async Task<IQueryable<SupervisedComputer>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).IncludeDetails();
        }
    }

    public static class IncludeDetailsExtension
    {
        public static IQueryable<SupervisedComputer> IncludeDetails(this IQueryable<SupervisedComputer> queryable)
        {
            return queryable
                .Include(e => e.SnitchingLogs)
                .Include(e => e.ActivityRecords)
                .Include(e => e.Calendars)
                .Include(e => e.KnownProcesses)
                .AsSplitQuery();
        }
    }
}