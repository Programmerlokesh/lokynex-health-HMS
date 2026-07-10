using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.DoctorOpd.Commands.CreatePrescription;
using LokynexHealth.Application.Features.DoctorOpd.Queries.GetPrescriptionByVisitId;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/prescriptions")]
public class PrescriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PrescriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetByVisitId), new { visitId = command.VisitId }, new { id });
    }

    [HttpGet("by-visit/{visitId:guid}")]
    public async Task<IActionResult> GetByVisitId(Guid visitId)
    {
        var prescription = await _mediator.Send(new GetPrescriptionByVisitIdQuery { VisitId = visitId });
        return prescription is null ? NotFound() : Ok(prescription);
    }
}