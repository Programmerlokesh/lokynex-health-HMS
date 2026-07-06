using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class PatientInsurance : BaseEntity
{
    public Guid PatientId { get; set; }
    public InsuranceType InsuranceType { get; set; }
    public string? SchemeNumber { get; set; }
    public string? TpaName { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public bool EligibilityVerified { get; set; }
    public DateTimeOffset? EligibilityCheckedAt { get; set; }
}
