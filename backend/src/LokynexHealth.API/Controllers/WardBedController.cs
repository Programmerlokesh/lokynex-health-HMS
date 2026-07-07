using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.WardBed.Commands.CreateWard;
using LokynexHealth.Application.Features.WardBed.Commands.CreateBed;
using LokynexHealth.Application.Features.WardBed.Commands.AdmitPatient;
using LokynexHealth.Application.Features.WardBed.Commands.TransferBed;
using LokynexHealth.Application.Features.WardBed.Commands.DischargePatient;
using LokynexHealth.Application.Features.WardBed.Commands.RecordNursingAssessment;
using LokynexHealth.Application.Features.WardBed.Commands.CompleteHousekeepingTask;
using LokynexHealth.Application.Features.WardBed.Queries.GetBedMap;
using LokynexHealth.Application.Features.WardBed.Queries.GetAdmissionById;
using LokynexHealth.Application.Features.WardBed.Queries.GetAdmissionsByPatient;
using LokynexHealth.Application.Features.WardBed.Queries.GetNursingAssessmentsByAdmission;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/ward-bed")]
public class WardBedController : ControllerBase
{
    private readonly IMediator _mediator;

    public WardBedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("wards")]
    public async Task<IActionResult> CreateWard([FromBody] CreateWardCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBedMap), new { id }, new { id });
    }

    [HttpPost("beds")]
    public async Task<IActionResult> CreateBed([FromBody] CreateBedCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBedMap), new { id }, new { id });
    }

    [HttpGet("bed-map")]
    public async Task<IActionResult> GetBedMap()
    {
        var map = await _mediator.Send(new GetBedMapQuery());
        return Ok(map);
    }

    [HttpPost("admissions")]
    public async Task<IActionResult> AdmitPatient([FromBody] AdmitPatientCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAdmissionById), new { id }, new { id });
    }

    [HttpGet("admissions/{id:guid}")]
    public async Task<IActionResult> GetAdmissionById(Guid id)
    {
        var admission = await _mediator.Send(new GetAdmissionByIdQuery { Id = id });
        return admission is null ? NotFound() : Ok(admission);
    }

    [HttpGet("admissions")]
    public async Task<IActionResult> GetAdmissionsByPatient([FromQuery] Guid patientId)
    {
        var admissions = await _mediator.Send(new GetAdmissionsByPatientQuery { PatientId = patientId });
        return Ok(admissions);
    }

    [HttpPost("admissions/{id:guid}/transfer")]
    public async Task<IActionResult> TransferBed(Guid id, [FromBody] TransferBedCommand command)
    {
        command.AdmissionId = id;
        var transferId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAdmissionById), new { id }, new { id = transferId });
    }

    [HttpPost("admissions/{id:guid}/discharge")]
    public async Task<IActionResult> DischargePatient(Guid id)
    {
        var admissionId = await _mediator.Send(new DischargePatientCommand { AdmissionId = id });
        return Ok(new { id = admissionId });
    }

    [HttpPost("admissions/{id:guid}/nursing-assessments")]
    public async Task<IActionResult> RecordNursingAssessment(Guid id, [FromBody] RecordNursingAssessmentCommand command)
    {
        command.AdmissionId = id;
        var assessmentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetNursingAssessmentsByAdmission), new { id }, new { id = assessmentId });
    }

    [HttpGet("admissions/{id:guid}/nursing-assessments")]
    public async Task<IActionResult> GetNursingAssessmentsByAdmission(Guid id)
    {
        var assessments = await _mediator.Send(new GetNursingAssessmentsByAdmissionQuery { AdmissionId = id });
        return Ok(assessments);
    }

    [HttpPost("housekeeping-tasks/{taskId:guid}/complete")]
    public async Task<IActionResult> CompleteHousekeepingTask(Guid taskId)
    {
        var id = await _mediator.Send(new CompleteHousekeepingTaskCommand { TaskId = taskId });
        return Ok(new { id });
    }
}