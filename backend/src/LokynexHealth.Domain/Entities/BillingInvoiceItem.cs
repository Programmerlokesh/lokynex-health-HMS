using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class BillingInvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string SourceModule { get; set; } = string.Empty;
    public Guid? SourceRecordId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal GstRatePct { get; set; }
    public decimal LineTotal { get; set; }
}
