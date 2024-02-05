using System.Threading.Tasks;

namespace SnitcherPortal.Data;

public interface ISnitcherPortalDbSchemaMigrator
{
    Task MigrateAsync();
}
