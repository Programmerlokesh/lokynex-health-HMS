using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class HousekeepingTask : BaseEntity
{
    public Guid BedId { get; set; }
    public string TaskType { get; set; } = "terminal_cleaning";
    public string Status { get; set; } = "pending";
    public Guid? AssignedTo { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}