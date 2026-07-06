using MediatR;
using LokynexHealth.Application.Features.Billing.DTOs;

namespace LokynexHealth.Application.Features.Billing.Queries.GetInvoiceById;

public class GetInvoiceByIdQuery : IRequest<InvoiceDto?>
{
    public Guid Id { get; set; }
}
