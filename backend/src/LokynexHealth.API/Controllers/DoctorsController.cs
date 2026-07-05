using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Doctors.Commands.CreateDoctor;
using LokynexHealth.Application.Features.Doctors.Queries.GetAllDoctors;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorCommand command)
    {
        var doctorId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAllDoctors), new { id = doctorId }, new { id = doctorId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDoctors([FromQuery] Guid tenantId)
    {
        var doctors = await _mediator.Send(new GetAllDoctorsQuery { TenantId = tenantId });
        return Ok(doctors);
    }
}
