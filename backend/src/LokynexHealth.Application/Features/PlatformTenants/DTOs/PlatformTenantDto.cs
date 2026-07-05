namespace LokynexHealth.Application.Features.PlatformTenants.DTOs;

public class PlatformTenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string DbSchemaName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
