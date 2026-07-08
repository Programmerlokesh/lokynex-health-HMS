using MediatR;

namespace LokynexHealth.Application.Features.Pharmacy.Commands.AddStockBatch;

public class AddStockBatchCommand : IRequest<Guid>
{
    public Guid DrugId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly ExpiryDate { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? Mrp { get; set; }
    public string? SupplierName { get; set; }
}