using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Billing.Commands.CreateInvoice;
using LokynexHealth.Application.Features.Billing.Commands.AddInvoiceItem;
using LokynexHealth.Application.Features.Billing.Commands.RecordPayment;
using LokynexHealth.Application.Features.Billing.Queries.GetInvoiceById;
using LokynexHealth.Application.Features.Billing.Queries.GetAllInvoicesByPatient;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
    {
        var invoiceId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvoiceById), new { id = invoiceId }, new { id = invoiceId });
    }

    [HttpPost("invoices/{id}/items")]
    public async Task<IActionResult> AddInvoiceItem(Guid id, [FromBody] AddInvoiceItemCommand command)
    {
        command.InvoiceId = id;
        var itemId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvoiceById), new { id }, new { id = itemId });
    }

    [HttpPost("invoices/{id}/payments")]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordPaymentCommand command)
    {
        command.InvoiceId = id;
        var paymentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvoiceById), new { id }, new { id = paymentId });
    }

    [HttpGet("invoices/{id}")]
    public async Task<IActionResult> GetInvoiceById(Guid id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery { Id = id });
        return invoice is null ? NotFound() : Ok(invoice);
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> GetAllInvoicesByPatient([FromQuery] Guid patientId)
    {
        var invoices = await _mediator.Send(new GetAllInvoicesByPatientQuery { PatientId = patientId });
        return Ok(invoices);
    }
}
