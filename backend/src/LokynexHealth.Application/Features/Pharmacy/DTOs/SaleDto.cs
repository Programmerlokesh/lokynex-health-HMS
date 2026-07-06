namespace LokynexHealth.Application.Features.Pharmacy.DTOs;

public class SaleDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? PatientId { get; set; }
    public Guid? PrescriptionId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public bool EwayBillTriggered { get; set; }
    public string? EwayBillNumber { get; set; }
    public DateTimeOffset SoldAt { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}