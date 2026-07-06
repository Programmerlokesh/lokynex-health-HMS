using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LokynexHealth.Infrastructure.Persistence;

public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (context is LokynexHealthDbContext tenantContext)
        {
            return (context.GetType(), tenantContext.SchemaName, designTime);
        }

        return (context.GetType(), designTime);
    }
}
