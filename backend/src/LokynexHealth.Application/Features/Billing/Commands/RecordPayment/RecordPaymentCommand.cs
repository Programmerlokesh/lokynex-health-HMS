using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.Billing.Commands.RecordPayment;

public class RecordPaymentCommand : IRequest<Guid>
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ReferenceNumber { get; set; }
}
