using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Pharmacy.Commands.CreateDrug;
using LokynexHealth.Application.Features.Pharmacy.Commands.AddStockBatch;
using LokynexHealth.Application.Features.Pharmacy.Commands.CreateSale;
using LokynexHealth.Application.Features.Pharmacy.Queries.GetAllDrugs;
using LokynexHealth.Application.Features.Pharmacy.Queries.GetStockByDrug;
using LokynexHealth.Application.Features.Pharmacy.Queries.GetSaleById;
using LokynexHealth.Application.Features.Pharmacy.Queries.GetAllSalesByPatient;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/pharmacy-pos")]
public class PharmacyPosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyPosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("drugs")]
    public async Task<IActionResult> CreateDrug([FromBody] CreateDrugCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAllDrugs), new { id }, new { id });
    }

    [HttpGet("drugs")]
    public async Task<IActionResult> GetAllDrugs([FromQuery] bool includeInactive = false)
    {
        var drugs = await _mediator.Send(new GetAllDrugsQuery { IncludeInactive = includeInactive });
        return Ok(drugs);
    }

    [HttpPost("stock-batches")]
    public async Task<IActionResult> AddStockBatch([FromBody] AddStockBatchCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetStockByDrug), new { drugId = command.DrugId }, new { id });
    }

    [HttpGet("drugs/{drugId:guid}/stock")]
    public async Task<IActionResult> GetStockByDrug(Guid drugId)
    {
        var stock = await _mediator.Send(new GetStockByDrugQuery { DrugId = drugId });
        return Ok(stock);
    }

    [HttpPost("sales")]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSaleById), new { id }, new { id });
    }

    [HttpGet("sales/{id:guid}")]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery { Id = id });
        return sale is null ? NotFound() : Ok(sale);
    }

    [HttpGet("sales")]
    public async Task<IActionResult> GetAllSalesByPatient([FromQuery] Guid patientId)
    {
        var sales = await _mediator.Send(new GetAllSalesByPatientQuery { PatientId = patientId });
        return Ok(sales);
    }
}