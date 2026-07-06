using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class BillingInvoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid? AdmissionId { get; set; }
    public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? EwayBillNumber { get; set; }
}
