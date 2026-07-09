using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.DoctorOpd.Commands.CreateOpdVisit;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/opd-visits")]
public class OpdVisitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OpdVisitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOpdVisit([FromBody] CreateOpdVisitCommand command)
    {
        var id = await _mediator.Send(command);
        return Created($"api/opd-visits/{id}", new { id });
    }
}
