namespace LokynexHealth.Application.Features.Pharmacy.DTOs;

public class SaleItemDto
{
    public Guid Id { get; set; }
    public Guid DrugId { get; set; }
    public Guid BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GstAmount { get; set; }
    public decimal LineTotal { get; set; }
}