using System.Text.RegularExpressions;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Infrastructure.Persistence;
using LokynexHealth.Infrastructure.Persistence.Platform;
using Microsoft.EntityFrameworkCore;

namespace LokynexHealth.IntegrationTests.Schema;

public class DocsSchemaCoverageTests
{
    [Fact]
    public void TenantDbContext_MapsEveryDocumentedHmsTable()
    {
        using var context = CreateTenantContext();

        var documentedTables = ReadDocumentedTables("hms");
        var mappedTables = context.Model.GetEntityTypes()
            .Where(entity => string.Equals(entity.GetSchema(), "hms", StringComparison.OrdinalIgnoreCase))
            .Select(entity => entity.GetTableName())
            .Where(table => !string.IsNullOrWhiteSpace(table))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingTables = documentedTables.Except(mappedTables, StringComparer.OrdinalIgnoreCase).ToList();

        Assert.Empty(missingTables);
    }

    [Fact]
    public void PlatformDbContext_MapsEveryDocumentedSuperadminTable()
    {
        using var context = CreatePlatformContext();

        var documentedTables = ReadDocumentedTables("superadmin");
        var mappedTables = context.Model.GetEntityTypes()
            .Where(entity => string.Equals(entity.GetSchema(), "superadmin", StringComparison.OrdinalIgnoreCase))
            .Select(entity => entity.GetTableName())
            .Where(table => !string.IsNullOrWhiteSpace(table))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingTables = documentedTables.Except(mappedTables, StringComparer.OrdinalIgnoreCase).ToList();

        Assert.Empty(missingTables);
    }

    private static LokynexHealthDbContext CreateTenantContext()
    {
        var options = new DbContextOptionsBuilder<LokynexHealthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LokynexHealthDbContext(options, new TestTenantContext());
    }

    private static PlatformDbContext CreatePlatformContext()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PlatformDbContext(options);
    }

    private static IReadOnlySet<string> ReadDocumentedTables(string schemaName)
    {
        var docsDirectory = FindDocsDirectory();
        var tables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var createTableRegex = new Regex(
            @"^\s*CREATE\s+TABLE\s+(?:IF\s+NOT\s+EXISTS\s+)?(?<table>""?[\w.]+""?)\s*\(",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        foreach (var file in Directory.EnumerateFiles(docsDirectory, "*.sql"))
        {
            foreach (var rawLine in File.ReadLines(file))
            {
                var match = createTableRegex.Match(rawLine);
                if (!match.Success)
                {
                    continue;
                }

                var qualifiedName = match.Groups["table"].Value.Trim().Trim('"');
                var parts = qualifiedName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 2 || !string.Equals(parts[0].Trim('"'), schemaName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                tables.Add(parts[1].Trim('"'));
            }
        }

        return tables;
    }

    private static string FindDocsDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "docs", "db-schema-reference");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate docs/db-schema-reference.");
    }

    private sealed class TestTenantContext : ITenantContext
    {
        public string? SchemaName { get; private set; } = "hms";
        public Guid? TenantId { get; private set; }

        public void SetTenant(Guid tenantId, string schemaName)
        {
            TenantId = tenantId;
            SchemaName = schemaName;
        }
    }
}
