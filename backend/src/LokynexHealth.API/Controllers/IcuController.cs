using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.IcuMonitoring.Commands.CreateIcuAdmission;
using LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordVitals;
using LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordVentilatorReading;
using LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordIoChart;
using LokynexHealth.Application.Features.IcuMonitoring.Commands.DischargeIcuAdmission;
using LokynexHealth.Application.Features.IcuMonitoring.Queries.GetIcuAdmissionById;
using LokynexHealth.Application.Features.IcuMonitoring.Queries.GetActiveIcuAdmissions;
using LokynexHealth.Application.Features.IcuMonitoring.Queries.GetVitalsByAdmission;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/icu")]
public class IcuController : ControllerBase
{
    private readonly IMediator _mediator;

    public IcuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("admissions")]
    public async Task<IActionResult> CreateIcuAdmission([FromBody] CreateIcuAdmissionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetIcuAdmissionById), new { id }, new { id });
    }

    [HttpGet("admissions/{id:guid}")]
    public async Task<IActionResult> GetIcuAdmissionById(Guid id)
    {
        var admission = await _mediator.Send(new GetIcuAdmissionByIdQuery { Id = id });
        return admission is null ? NotFound() : Ok(admission);
    }

    [HttpGet("admissions/active")]
    public async Task<IActionResult> GetActiveIcuAdmissions()
    {
        var admissions = await _mediator.Send(new GetActiveIcuAdmissionsQuery());
        return Ok(admissions);
    }

    [HttpPost("admissions/{id:guid}/vitals")]
    public async Task<IActionResult> RecordVitals(Guid id, [FromBody] RecordVitalsCommand command)
    {
        command.IcuAdmissionId = id;
        var vitalId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetVitalsByAdmission), new { id }, new { id = vitalId });
    }

    [HttpGet("admissions/{id:guid}/vitals")]
    public async Task<IActionResult> GetVitalsByAdmission(Guid id)
    {
        var vitals = await _mediator.Send(new GetVitalsByAdmissionQuery { IcuAdmissionId = id });
        return Ok(vitals);
    }

    [HttpPost("admissions/{id:guid}/ventilator")]
    public async Task<IActionResult> RecordVentilatorReading(Guid id, [FromBody] RecordVentilatorReadingCommand command)
    {
        command.IcuAdmissionId = id;
        var recordId = await _mediator.Send(command);
        return Ok(new { id = recordId });
    }

    [HttpPost("admissions/{id:guid}/io-chart")]
    public async Task<IActionResult> RecordIoChart(Guid id, [FromBody] RecordIoChartCommand command)
    {
        command.IcuAdmissionId = id;
        var chartId = await _mediator.Send(command);
        return Ok(new { id = chartId });
    }

    [HttpPost("admissions/{id:guid}/discharge")]
    public async Task<IActionResult> DischargeIcuAdmission(Guid id)
    {
        await _mediator.Send(new DischargeIcuAdmissionCommand { IcuAdmissionId = id });
        return NoContent();
    }
}