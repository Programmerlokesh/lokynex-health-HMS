using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Laboratory.Commands.CreateLabTest;
using LokynexHealth.Application.Features.Laboratory.Commands.CreateLabOrder;
using LokynexHealth.Application.Features.Laboratory.Commands.CollectSample;
using LokynexHealth.Application.Features.Laboratory.Commands.EnterResult;
using LokynexHealth.Application.Features.Laboratory.Commands.ReleaseLabOrder;
using LokynexHealth.Application.Features.Laboratory.Queries.GetAllLabTests;
using LokynexHealth.Application.Features.Laboratory.Queries.GetLabOrderById;
using LokynexHealth.Application.Features.Laboratory.Queries.GetLabOrdersByPatient;
using LokynexHealth.Application.Features.Laboratory.Queries.GetPendingLabOrders;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/lab")]
public class LabController : ControllerBase
{
    private readonly IMediator _mediator;

    public LabController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("tests")]
    public async Task<IActionResult> CreateLabTest([FromBody] CreateLabTestCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAllLabTests), new { id }, new { id });
    }

    [HttpGet("tests")]
    public async Task<IActionResult> GetAllLabTests([FromQuery] bool includeInactive = false)
    {
        var tests = await _mediator.Send(new GetAllLabTestsQuery { IncludeInactive = includeInactive });
        return Ok(tests);
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateLabOrder([FromBody] CreateLabOrderCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLabOrderById), new { id }, new { id });
    }

    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> GetLabOrderById(Guid id)
    {
        var order = await _mediator.Send(new GetLabOrderByIdQuery { Id = id });
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetLabOrdersByPatient([FromQuery] Guid patientId)
    {
        var orders = await _mediator.Send(new GetLabOrdersByPatientQuery { PatientId = patientId });
        return Ok(orders);
    }

    [HttpGet("orders/pending")]
    public async Task<IActionResult> GetPendingLabOrders()
    {
        var orders = await _mediator.Send(new GetPendingLabOrdersQuery());
        return Ok(orders);
    }

    [HttpPost("samples/collect")]
    public async Task<IActionResult> CollectSample([FromBody] CollectSampleCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLabOrderById), new { id = command.OrderId }, new { id });
    }

    [HttpPost("results")]
    public async Task<IActionResult> EnterResult([FromBody] EnterResultCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpPost("orders/{id:guid}/release")]
    public async Task<IActionResult> ReleaseLabOrder(Guid id)
    {
        await _mediator.Send(new ReleaseLabOrderCommand { OrderId = id });
        return NoContent();
    }
}