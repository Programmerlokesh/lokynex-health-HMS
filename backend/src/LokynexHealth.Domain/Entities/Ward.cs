using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class Ward : BaseEntity
{
    public string WardName { get; set; } = string.Empty;
    public string? WardType { get; set; }
    public string? Floor { get; set; }
    public bool IsActive { get; set; } = true;
}