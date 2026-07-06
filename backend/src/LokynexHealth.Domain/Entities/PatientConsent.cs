using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;
using System.Net;

namespace LokynexHealth.Domain.Entities;

public class PatientConsent : BaseEntity
{
    public Guid PatientId { get; set; }
    public string ConsentType { get; set; } = string.Empty;
    public ConsentStatus Status { get; set; } = ConsentStatus.Granted;
    public DateTimeOffset GrantedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public string? CapturedVia { get; set; }
    public IPAddress? IpAddress { get; set; }
}
