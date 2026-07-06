namespace LokynexHealth.Application.Features.Billing.DTOs;

public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public string SourceModule { get; set; } = string.Empty;
    public Guid? SourceRecordId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GstRatePct { get; set; }
    public decimal LineTotal { get; set; }
}
