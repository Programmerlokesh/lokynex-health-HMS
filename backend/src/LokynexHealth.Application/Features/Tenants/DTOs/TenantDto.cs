namespace LokynexHealth.Application.Features.Tenants.DTOs;

public class TenantDto
{
    public Guid Id { get; set; }
    public string HospitalName { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubscriptionStartDate { get; set; }
}
