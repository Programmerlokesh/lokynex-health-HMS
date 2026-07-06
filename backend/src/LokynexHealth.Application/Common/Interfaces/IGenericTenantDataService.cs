using System.Text.Json;
using LokynexHealth.Application.Common.Models;

namespace LokynexHealth.Application.Common.Interfaces;

public interface IGenericTenantDataService
{
    Task<IReadOnlyList<TenantTableDto>> GetTablesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Dictionary<string, object?>>> ListAsync(
        string tableName,
        int limit,
        int offset,
        CancellationToken cancellationToken);
    Task<Dictionary<string, object?>?> GetByIdAsync(string tableName, Guid id, CancellationToken cancellationToken);
    Task<Guid> CreateAsync(string tableName, JsonElement payload, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(string tableName, Guid id, JsonElement payload, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string tableName, Guid id, CancellationToken cancellationToken);
}
