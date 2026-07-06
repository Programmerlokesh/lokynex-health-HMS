using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class BillingRateMaster : BaseEntity
{
    public string SchemeName { get; set; } = string.Empty;
    public string? PackageCode { get; set; }
    public string? ServiceDescription { get; set; }
    public decimal Rate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}
