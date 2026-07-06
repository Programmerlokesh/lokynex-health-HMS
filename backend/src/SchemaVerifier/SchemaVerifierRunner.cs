using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using LokynexHealth.Infrastructure.Persistence;
using System.Linq;

namespace LokynexHealth.SchemaVerifier;

public static class SchemaVerifierRunner
{
    public static int Run()
    {
        Console.WriteLine("Schema verifier starting...");

        // Locate docs directory by walking up until we find it
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        DirectoryInfo? repoDir = null;
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "docs", "db-schema-reference");
            if (Directory.Exists(candidate))
            {
                repoDir = dir;
                break;
            }

            dir = dir.Parent;
        }

        if (repoDir == null)
        {
            Console.WriteLine("Could not locate docs/db-schema-reference in parent directories.");
            return 1;
        }

        var repoRoot = repoDir.FullName;
        var docsDir = Path.Combine(repoRoot, "docs", "db-schema-reference");

        var sqlFiles = Directory.GetFiles(docsDir, "*.sql");
        var tableDefinitions = ParseSqlTables(sqlFiles);

        Console.WriteLine($"Parsed {tableDefinitions.Count} tables from SQL files.");

        // Build DbContext model using InMemory provider
        var optionsBuilder = new DbContextOptionsBuilder<LokynexHealthDbContext>();
        optionsBuilder.UseInMemoryDatabase("_schema_verifier_db");

        var tenantContext = new SimpleTenantContext();

        using var context = new LokynexHealthDbContext(optionsBuilder.Options, tenantContext);
        var model = context.Model;

        var mismatches = new List<string>();

        foreach (var entityType in model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (string.IsNullOrEmpty(tableName)) continue;
            if (!tableDefinitions.TryGetValue(tableName, out var sqlCols))
            {
                // try fallback: find any schema-qualified table that ends with .{tableName}
                var fallback = tableDefinitions.FirstOrDefault(kvp => kvp.Key.EndsWith("." + tableName, StringComparison.OrdinalIgnoreCase));
                if (!fallback.Equals(default(KeyValuePair<string, Dictionary<string, string>>)))
                {
                    sqlCols = fallback.Value;
                }
                else
                {
                    mismatches.Add($"Table '{tableName}' exists in EF model but not in SQL docs.");
                    continue;
                }
            }

            foreach (var prop in entityType.GetProperties())
            {
                var colName = prop.GetColumnName(StoreObjectIdentifier.Table(tableName, entityType.GetSchema()));
                if (string.IsNullOrEmpty(colName)) colName = prop.Name;
                if (!sqlCols.ContainsKey(colName))
                {
                    mismatches.Add($"Column '{colName}' on table '{tableName}' missing in SQL docs (EF property: {prop.Name}).");
                }
            }
        }

        // Check SQL tables not present in EF
        foreach (var sqlTable in tableDefinitions.Keys)
        {
            // if sqlTable is schema-qualified, consider its bare name too
            var bare = sqlTable.Contains('.') ? sqlTable.Split('.').Last() : sqlTable;
            var efHas = model.GetEntityTypes().Any(e => string.Equals(e.GetTableName(), bare, StringComparison.OrdinalIgnoreCase));
            if (!efHas) mismatches.Add($"Table '{sqlTable}' exists in SQL docs but not in EF model.");
        }

        if (mismatches.Count == 0)
        {
            Console.WriteLine("No differences found between SQL docs and EF model (columns/tables presence).\n");
            return 0;
        }

        Console.WriteLine("Differences found:");
        foreach (var m in mismatches) Console.WriteLine(" - " + m);

        // Write report
        var reportPath = Path.Combine(repoRoot, "backend", "src", "SchemaVerifier", "report.txt");
        File.WriteAllLines(reportPath, mismatches);
        Console.WriteLine($"Report written to {reportPath}");
        return 0;
    }

    internal class SimpleTenantContext : LokynexHealth.Application.Common.Interfaces.ITenantContext
    {
        private Guid? _tenantId;
        private string? _schemaName = "hms_default";

        public string? SchemaName => _schemaName;
        public Guid? TenantId => _tenantId;

        public void SetTenant(Guid tenantId, string schemaName)
        {
            _tenantId = tenantId;
            _schemaName = schemaName;
        }
    }

    private static Dictionary<string, Dictionary<string, string>> ParseSqlTables(IEnumerable<string> sqlFiles)
    {
        var tableDefinitions = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        var createTableRegex = new Regex(@"^\s*CREATE\s+TABLE\s+(?:IF\s+NOT\s+EXISTS\s+)?(?<table>""?[\w.]+""?)\s*\(", RegexOptions.IgnoreCase);
        var columnRegex = new Regex(@"^""?(?<name>[A-Za-z_][A-Za-z0-9_]*)""?\s+(?<type>.+)$", RegexOptions.IgnoreCase);
        var constraintKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CONSTRAINT",
            "PRIMARY",
            "FOREIGN",
            "UNIQUE",
            "CHECK",
            "EXCLUDE"
        };

        foreach (var file in sqlFiles)
        {
            string? currentTable = null;
            Dictionary<string, string>? columns = null;

            foreach (var rawLine in File.ReadLines(file))
            {
                var line = StripLineComment(rawLine).Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                if (currentTable is null)
                {
                    var match = createTableRegex.Match(line);
                    if (!match.Success)
                    {
                        continue;
                    }

                    currentTable = match.Groups["table"].Value.Trim().Trim('"');
                    columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                if (line.StartsWith(");", StringComparison.Ordinal))
                {
                    tableDefinitions[currentTable] = columns!;
                    currentTable = null;
                    columns = null;
                    continue;
                }

                line = line.TrimEnd(',');
                var firstToken = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (firstToken is null || constraintKeywords.Contains(firstToken))
                {
                    continue;
                }

                var columnMatch = columnRegex.Match(line);
                if (!columnMatch.Success)
                {
                    continue;
                }

                var columnName = columnMatch.Groups["name"].Value.Trim().Trim('"');
                var columnType = columnMatch.Groups["type"].Value.Trim();
                columns!.TryAdd(columnName, columnType);
            }
        }

        return tableDefinitions;
    }

    private static string StripLineComment(string line)
    {
        var commentStart = line.IndexOf("--", StringComparison.Ordinal);
        return commentStart >= 0 ? line[..commentStart] : line;
    }
}
