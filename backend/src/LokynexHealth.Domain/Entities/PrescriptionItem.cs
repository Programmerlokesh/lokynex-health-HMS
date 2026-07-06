using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class PrescriptionItem : BaseEntity
{
    public Guid PrescriptionId { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string? RxnormCode { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public short? DurationDays { get; set; }
    public ScheduleDrugFlag ScheduleFlag { get; set; } = ScheduleDrugFlag.None;
    public bool InteractionChecked { get; set; }
    public string? InteractionWarning { get; set; }
    public string? AllergyWarning { get; set; }
    public bool AiAutocompleted { get; set; }
    public Guid? AiLogId { get; set; }
}
