using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Infrastructure.Persistence;

namespace LokynexHealth.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly string _connectionString;

    public TenantProvisioningService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public async Task ProvisionTenantSchemaAsync(string schemaName, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", connection);

        await command.ExecuteNonQueryAsync(cancellationToken);

        await MigrateTenantSchemaAsync(schemaName, cancellationToken);
    }

    private async Task MigrateTenantSchemaAsync(string schemaName, CancellationToken cancellationToken)
    {
        var options = new DbContextOptionsBuilder<LokynexHealthDbContext>()
            .UseNpgsql(_connectionString)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        var fixedTenantContext = new ProvisioningTenantContext(schemaName);

        await using var context = new LokynexHealthDbContext(options, fixedTenantContext);
        await context.Database.MigrateAsync(cancellationToken);
    }

    private class ProvisioningTenantContext : ITenantContext
    {
        public string? SchemaName { get; }
        public Guid? TenantId => null;

        public ProvisioningTenantContext(string schemaName)
        {
            SchemaName = schemaName;
        }

        public void SetTenant(Guid tenantId, string schemaName)
        {
        }
    }
}
