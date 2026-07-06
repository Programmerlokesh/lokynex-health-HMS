namespace LokynexHealth.Application.Common.Models;

public class HmsModuleDto
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<string> DependsOn { get; set; } = new();
    public List<ModuleTableDto> Tables { get; set; } = new();
}

public class ModuleTableDto
{
    public string Name { get; set; } = string.Empty;
    public List<ModuleColumnDto> Columns { get; set; } = new();
}

public class ModuleColumnDto
{
    public string Name { get; set; } = string.Empty;
    public string SqlType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool HasDefault { get; set; }
    public bool IsPrimaryKey { get; set; }
}
