using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.Pharmacy.Commands.CreateSale;

public class SaleItemInput
{
    public Guid DrugId { get; set; }
    public decimal Quantity { get; set; }

    /// <summary>Optional. If not provided, FEFO (earliest expiry) batch(es) are auto-selected.</summary>
    public Guid? BatchId { get; set; }
}

public class CreateSaleCommand : IRequest<Guid>
{
    public Guid? PatientId { get; set; }
    public Guid? PrescriptionId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public List<SaleItemInput> Items { get; set; } = new();
}