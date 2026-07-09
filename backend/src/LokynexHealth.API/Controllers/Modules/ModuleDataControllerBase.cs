using System.Text.Json;
using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

public abstract class ModuleDataControllerBase : ControllerBase
{
    private readonly IGenericTenantDataService _dataService;

    protected ModuleDataControllerBase(IGenericTenantDataService dataService)
    {
        _dataService = dataService;
    }

    protected abstract string ModuleKey { get; }

    [HttpGet("{tableName}")]
    public async Task<IActionResult> ListRows(
        string tableName,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _dataService.ListAsync(tableName, limit, offset, cancellationToken));
    }

    [HttpGet("{tableName}/{id:guid}")]
    public async Task<IActionResult> GetById(
        string tableName,
        Guid id,
        CancellationToken cancellationToken)
    {
        var row = await _dataService.GetByIdAsync(tableName, id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost("{tableName}")]
    public async Task<IActionResult> Create(
        string tableName,
        [FromBody] JsonElement payload,
        CancellationToken cancellationToken)
    {
        var id = await _dataService.CreateAsync(tableName, payload, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { tableName, id }, new { id });
    }

    [HttpPatch("{tableName}/{id:guid}")]
    public async Task<IActionResult> Update(
        string tableName,
        Guid id,
        [FromBody] JsonElement payload,
        CancellationToken cancellationToken)
    {
        var updated = await _dataService.UpdateAsync(tableName, id, payload, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{tableName}/{id:guid}")]
    public async Task<IActionResult> Delete(
        string tableName,
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _dataService.DeleteAsync(tableName, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}