using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Billing.Commands.AddInvoiceItem;

public class AddInvoiceItemCommandHandler : IRequestHandler<AddInvoiceItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddInvoiceItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddInvoiceItemCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.BillingInvoices
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            throw new KeyNotFoundException($"Invoice with Id '{request.InvoiceId}' was not found.");
        }

        var lineTotal = request.Quantity * request.UnitPrice;

        var item = new BillingInvoiceItem
        {
            InvoiceId = request.InvoiceId,
            SourceModule = request.SourceModule,
            SourceRecordId = request.SourceRecordId,
            Description = request.Description,
            HsnSacCode = request.HsnSacCode,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            GstRatePct = request.GstRatePct,
            LineTotal = lineTotal
        };

        _context.BillingInvoiceItems.Add(item);

        // Recompute invoice-level totals from all items (intra-state: GST split equally into CGST + SGST)
        var allItems = await _context.BillingInvoiceItems
            .Where(i => i.InvoiceId == request.InvoiceId)
            .ToListAsync(cancellationToken);

        var subtotal = allItems.Sum(i => i.LineTotal) + lineTotal;
        var totalGst = allItems.Sum(i => i.LineTotal * i.GstRatePct / 100m) + (lineTotal * request.GstRatePct / 100m);

        invoice.Subtotal = subtotal;
        invoice.CgstAmount = totalGst / 2m;
        invoice.SgstAmount = totalGst / 2m;
        invoice.IgstAmount = 0;
        invoice.TotalAmount = subtotal - invoice.DiscountAmount + invoice.CgstAmount + invoice.SgstAmount + invoice.IgstAmount;

        await _context.SaveChangesAsync(cancellationToken);

        return item.Id;
    }
}
