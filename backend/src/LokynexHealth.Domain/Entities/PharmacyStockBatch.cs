using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class PharmacyStockBatch : BaseEntity
{
    public Guid DrugId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly ExpiryDate { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? Mrp { get; set; }
    public string? SupplierName { get; set; }
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
}