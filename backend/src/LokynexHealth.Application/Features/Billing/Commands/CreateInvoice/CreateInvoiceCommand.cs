using MediatR;

namespace LokynexHealth.Application.Features.Billing.Commands.CreateInvoice;

public class CreateInvoiceCommand : IRequest<Guid>
{
    public Guid PatientId { get; set; }
}
