using Microsoft.EntityFrameworkCore;
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
        // Step 1: Create the raw Postgres schema
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", connection);

        await command.ExecuteNonQueryAsync(cancellationToken);

        // Step 2: Run EF Core migrations against the new schema
        await MigrateTenantSchemaAsync(schemaName, cancellationToken);
    }

    private async Task MigrateTenantSchemaAsync(string schemaName, CancellationToken cancellationToken)
    {
        var options = new DbContextOptionsBuilder<LokynexHealthDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        var fixedTenantContext = new ProvisioningTenantContext(schemaName);

        await using var context = new LokynexHealthDbContext(options, fixedTenantContext);
        await context.Database.MigrateAsync(cancellationToken);
    }

    // Minimal fixed-value ITenantContext, used only during provisioning
    // to force the DbContext to target the newly created schema.
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
            // Not used in this fixed context
        }
    }
}
