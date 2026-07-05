using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Patients.Commands.CreatePatient;
using LokynexHealth.Application.Features.Patients.Commands.UpdatePatient;
using LokynexHealth.Application.Features.Patients.Queries.GetPatientById;
using LokynexHealth.Application.Features.Patients.Queries.GetAllPatients;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientCommand command)
    {
        var patientId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPatientById), new { id = patientId }, new { id = patientId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(Guid id)
    {
        var patient = await _mediator.Send(new GetPatientByIdQuery { Id = id });
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPatients([FromQuery] Guid tenantId)
    {
        var patients = await _mediator.Send(new GetAllPatientsQuery { TenantId = tenantId });
        return Ok(patients);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] UpdatePatientCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { error = "Route id and body id do not match." });
        }

        await _mediator.Send(command);
        return NoContent();
    }
}
