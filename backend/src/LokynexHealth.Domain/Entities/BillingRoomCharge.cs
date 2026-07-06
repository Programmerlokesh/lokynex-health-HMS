using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class BillingRoomCharge : BaseEntity
{
    public Guid AdmissionId { get; set; }
    public Guid BedId { get; set; }
    public DateTimeOffset ChargeFrom { get; set; }
    public DateTimeOffset ChargeTo { get; set; }
    public decimal DailyRate { get; set; }
    public decimal ComputedAmount { get; set; }
    public Guid? InvoiceItemId { get; set; }
}
