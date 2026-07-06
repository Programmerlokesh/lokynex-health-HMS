using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class PharmacySaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid DrugId { get; set; }
    public Guid BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GstAmount { get; set; }
    public decimal LineTotal { get; set; }
    public DateTimeOffset? InteractionCheckedAt { get; set; }
    public Guid? AiLogId { get; set; }
}