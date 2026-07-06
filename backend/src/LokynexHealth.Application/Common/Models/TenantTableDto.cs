namespace LokynexHealth.Application.Common.Models;

public class TenantTableDto
{
    public string Name { get; set; } = string.Empty;
    public List<TenantColumnDto> Columns { get; set; } = new();
}

public class TenantColumnDto
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool HasDefault { get; set; }
}
