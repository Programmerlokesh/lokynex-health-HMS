using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class PharmacyDrugCatalog : BaseEntity
{
    public string DrugName { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public bool IsJanAushadhiGeneric { get; set; }
    public string? HsnCode { get; set; }
    public decimal GstRatePct { get; set; } = 12.00m;
    public ScheduleDrugFlag ScheduleFlag { get; set; } = ScheduleDrugFlag.None;
    public string UnitOfMeasure { get; set; } = "strip";
    public bool IsActive { get; set; } = true;
}