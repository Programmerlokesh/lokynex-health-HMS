using MediatR;

namespace LokynexHealth.Application.Features.Laboratory.Commands.GenerateLabInvoice;

public class GenerateLabInvoiceCommand : IRequest<Guid>
{
    public Guid OrderId { get; set; }
    public decimal GstRatePct { get; set; } = 0;
}