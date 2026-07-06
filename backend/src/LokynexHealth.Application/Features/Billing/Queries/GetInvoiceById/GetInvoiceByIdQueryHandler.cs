using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Billing.DTOs;

namespace LokynexHealth.Application.Features.Billing.Queries.GetInvoiceById;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IApplicationDbContext _context;

    public GetInvoiceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _context.BillingInvoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (invoice is null)
        {
            return null;
        }

        var items = await _context.BillingInvoiceItems
            .AsNoTracking()
            .Where(i => i.InvoiceId == request.Id)
            .Select(i => new InvoiceItemDto
            {
                Id = i.Id,
                SourceModule = i.SourceModule,
                SourceRecordId = i.SourceRecordId,
                Description = i.Description,
                HsnSacCode = i.HsnSacCode,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                GstRatePct = i.GstRatePct,
                LineTotal = i.LineTotal
            })
            .ToListAsync(cancellationToken);

        var payments = await _context.BillingPayments
            .AsNoTracking()
            .Where(p => p.InvoiceId == request.Id)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                Method = p.Method.ToString(),
                ReferenceNumber = p.ReferenceNumber,
                PaidAt = p.PaidAt
            })
            .ToListAsync(cancellationToken);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PatientId = invoice.PatientId,
            InvoiceDate = invoice.InvoiceDate,
            Subtotal = invoice.Subtotal,
            DiscountAmount = invoice.DiscountAmount,
            CgstAmount = invoice.CgstAmount,
            SgstAmount = invoice.SgstAmount,
            IgstAmount = invoice.IgstAmount,
            TotalAmount = invoice.TotalAmount,
            PaymentStatus = invoice.PaymentStatus.ToString(),
            Items = items,
            Payments = payments
        };
    }
}
