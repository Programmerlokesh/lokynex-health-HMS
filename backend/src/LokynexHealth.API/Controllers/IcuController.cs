using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Icu.Commands.AdmitToIcu;
using LokynexHealth.Application.Features.Icu.Commands.RecordVitals;
using LokynexHealth.Application.Features.Icu.Commands.RecordVentilatorSettings;
using LokynexHealth.Application.Features.Icu.Commands.RecordIoChart;
using LokynexHealth.Application.Features.Icu.Commands.DischargeFromIcu;
using LokynexHealth.Application.Features.Icu.Queries.GetIcuAdmissionById;
using LokynexHealth.Application.Features.Icu.Queries.GetActiveIcuAdmissions;
using LokynexHealth.Application.Features.Icu.Queries.GetVitalsByIcuAdmission;
using LokynexHealth.Application.Features.Icu.Queries.GetIoChartsByIcuAdmission;

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
    public async Task<IActionResult> AdmitToIcu([FromBody] AdmitToIcuCommand command)
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

    [HttpPost("admissions/{id:guid}/discharge")]
    public async Task<IActionResult> DischargeFromIcu(Guid id)
    {
        var result = await _mediator.Send(new DischargeFromIcuCommand { IcuAdmissionId = id });
        return Ok(new { id = result });
    }

    [HttpPost("admissions/{id:guid}/vitals")]
    public async Task<IActionResult> RecordVitals(Guid id, [FromBody] RecordVitalsCommand command)
    {
        command.IcuAdmissionId = id;
        var vital = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetVitalsByIcuAdmission), new { id }, vital);
    }

    [HttpGet("admissions/{id:guid}/vitals")]
    public async Task<IActionResult> GetVitalsByIcuAdmission(Guid id)
    {
        var vitals = await _mediator.Send(new GetVitalsByIcuAdmissionQuery { IcuAdmissionId = id });
        return Ok(vitals);
    }

    [HttpPost("admissions/{id:guid}/ventilator")]
    public async Task<IActionResult> RecordVentilatorSettings(Guid id, [FromBody] RecordVentilatorSettingsCommand command)
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
        return CreatedAtAction(nameof(GetIoChartsByIcuAdmission), new { id }, new { id = chartId });
    }

    [HttpGet("admissions/{id:guid}/io-chart")]
    public async Task<IActionResult> GetIoChartsByIcuAdmission(Guid id)
    {
        var charts = await _mediator.Send(new GetIoChartsByIcuAdmissionQuery { IcuAdmissionId = id });
        return Ok(charts);
    }
}