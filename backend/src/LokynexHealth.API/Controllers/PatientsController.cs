using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Patients.Commands.CreatePatient;

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
        try
        {
            var patientId = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreatePatient), new { id = patientId }, new { id = patientId });
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }
    }
}
