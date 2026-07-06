using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Billing.DTOs;

namespace LokynexHealth.Application.Features.Billing.Queries.GetAllInvoicesByPatient;

public class GetAllInvoicesByPatientQueryHandler : IRequestHandler<GetAllInvoicesByPatientQuery, List<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllInvoicesByPatientQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvoiceDto>> Handle(GetAllInvoicesByPatientQuery request, CancellationToken cancellationToken)
    {
        // Note: this filter is by patientId, not tenantId — schema-per-tenant already
        // scopes the data, so this is a legitimate business filter (unlike the old
        // pre-Day-37 tenantId query param that GetAllPatients used to require).
        var invoices = await _context.BillingInvoices
            .AsNoTracking()
            .Where(i => i.PatientId == request.PatientId)
            .ToListAsync(cancellationToken);

        var invoiceIds = invoices.Select(i => i.Id).ToList();

        var itemsByInvoice = await _context.BillingInvoiceItems
            .AsNoTracking()
            .Where(i => invoiceIds.Contains(i.InvoiceId))
            .ToListAsync(cancellationToken);

        var paymentsByInvoice = await _context.BillingPayments
            .AsNoTracking()
            .Where(p => invoiceIds.Contains(p.InvoiceId))
            .ToListAsync(cancellationToken);

        return invoices.Select(invoice => new InvoiceDto
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
            Items = itemsByInvoice
                .Where(i => i.InvoiceId == invoice.Id)
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
                }).ToList(),
            Payments = paymentsByInvoice
                .Where(p => p.InvoiceId == invoice.Id)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    Method = p.Method.ToString(),
                    ReferenceNumber = p.ReferenceNumber,
                    PaidAt = p.PaidAt
                }).ToList()
        }).ToList();
    }
}
