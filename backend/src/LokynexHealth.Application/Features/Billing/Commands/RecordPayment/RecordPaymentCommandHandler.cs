using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Billing.Commands.RecordPayment;

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RecordPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.BillingInvoices
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            throw new KeyNotFoundException($"Invoice with Id '{request.InvoiceId}' was not found.");
        }

        var payment = new BillingPayment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            Method = request.Method,
            ReferenceNumber = request.ReferenceNumber
        };

        _context.BillingPayments.Add(payment);

        // Recompute PaymentStatus from ALL payments (existing + this new one) vs TotalAmount
        var existingPaidSum = await _context.BillingPayments
            .Where(p => p.InvoiceId == request.InvoiceId)
            .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

        var totalPaid = existingPaidSum + request.Amount;

        invoice.PaymentStatus = totalPaid <= 0
            ? PaymentStatus.Pending
            : totalPaid >= invoice.TotalAmount
                ? PaymentStatus.Paid
                : PaymentStatus.Partial;

        await _context.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
