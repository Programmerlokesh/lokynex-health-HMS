using MediatR;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetSaleById;

public class GetSaleByIdQuery : IRequest<SaleDto?>
{
    public Guid Id { get; set; }
}