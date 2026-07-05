using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Infrastructure.Services;

public class TenantContext : ITenantContext
{
    public string? SchemaName { get; private set; }
    public Guid? TenantId { get; private set; }

    public void SetTenant(Guid tenantId, string schemaName)
    {
        TenantId = tenantId;
        SchemaName = schemaName;
    }
}
