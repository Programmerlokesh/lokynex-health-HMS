using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class BillingClaim : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid? PatientInsuranceId { get; set; }
    public ClaimType ClaimType { get; set; }
    public string? ClaimNumber { get; set; }
    public decimal ClaimedAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SettledAt { get; set; }
}
