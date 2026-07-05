namespace LokynexHealth.Application.Common.Interfaces;

public interface ITenantContext
{
    string? SchemaName { get; }
    Guid? TenantId { get; }
    void SetTenant(Guid tenantId, string schemaName);
}
