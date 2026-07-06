using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class PharmacySale : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? PatientId { get; set; }
    public Guid? PrescriptionId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Paid;
    public PaymentMethod? PaymentMethod { get; set; }
    public bool EwayBillTriggered { get; set; }
    public string? EwayBillNumber { get; set; }
    public DateTimeOffset SoldAt { get; set; } = DateTimeOffset.UtcNow;
}