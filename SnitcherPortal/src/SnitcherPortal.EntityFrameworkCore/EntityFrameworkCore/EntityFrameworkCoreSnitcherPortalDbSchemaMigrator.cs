using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SnitcherPortal.Data;
using Volo.Abp.DependencyInjection;

namespace SnitcherPortal.EntityFrameworkCore;

public class EntityFrameworkCoreSnitcherPortalDbSchemaMigrator
    : ISnitcherPortalDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreSnitcherPortalDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the SnitcherPortalDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<SnitcherPortalDbContext>()
            .Database
            .MigrateAsync();
    }
}
