using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SnitcherPortal.Data;

/* This is used if database provider does't define
 * ISnitcherPortalDbSchemaMigrator implementation.
 */
public class NullSnitcherPortalDbSchemaMigrator : ISnitcherPortalDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
