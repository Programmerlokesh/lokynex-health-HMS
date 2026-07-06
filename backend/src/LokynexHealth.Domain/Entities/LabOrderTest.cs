using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class LabOrderTest : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid TestId { get; set; }
    public decimal PriceApplied { get; set; }
}