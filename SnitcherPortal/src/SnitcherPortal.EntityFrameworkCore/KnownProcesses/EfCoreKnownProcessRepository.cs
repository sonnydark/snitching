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

namespace SnitcherPortal.KnownProcesses
{
    public class EfCoreKnownProcessRepository : EfCoreRepository<SnitcherPortalDbContext, KnownProcess, Guid>, IKnownProcessRepository
    {
        public EfCoreKnownProcessRepository(IDbContextProvider<SnitcherPortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<KnownProcess>> GetListBySupervisedComputerIdAsync(
           Guid supervisedComputerId,
           string? sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? KnownProcessConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountBySupervisedComputerIdAsync(Guid supervisedComputerId, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(x => x.SupervisedComputerId == supervisedComputerId).CountAsync(cancellationToken);
        }

        public virtual async Task<List<KnownProcess>> GetListAsync(
            string? filterText = null,
            string? name = null,
            bool? isHidden = null,
            bool? isImportant = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, isHidden, isImportant);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? KnownProcessConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            bool? isHidden = null,
            bool? isImportant = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, name, isHidden, isImportant);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<KnownProcess> ApplyFilter(
            IQueryable<KnownProcess> query,
            string? filterText = null,
            string? name = null,
            bool? isHidden = null,
            bool? isImportant = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(isHidden.HasValue, e => e.IsHidden == isHidden)
                    .WhereIf(isImportant.HasValue, e => e.IsImportant == isImportant);
        }
    }
}