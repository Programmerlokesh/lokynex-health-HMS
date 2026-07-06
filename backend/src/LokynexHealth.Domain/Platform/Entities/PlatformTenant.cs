using LokynexHealth.Domain.Platform.Enums;

namespace LokynexHealth.Domain.Platform.Entities;

public class PlatformTenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public FacilityType FacilityType { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Gstin { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public int? BedCount { get; set; }
    public int? DoctorCount { get; set; }
    public string Subdomain { get; set; } = string.Empty;
    public string DbSchemaName { get; set; } = string.Empty;
    public string RlsTag { get; set; } = string.Empty;
    public Guid? ParentTenantId { get; set; }
    public TenantPlatformStatus Status { get; set; } = TenantPlatformStatus.Trial;
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
