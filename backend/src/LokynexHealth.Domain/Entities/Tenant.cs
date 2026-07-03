using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class Tenant : BaseEntity
{
    public string HospitalName { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;   // e.g. "apollo" -> apollo.lokynexhealth.com
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public TenantStatus Status { get; set; } = TenantStatus.PendingActivation;
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
}