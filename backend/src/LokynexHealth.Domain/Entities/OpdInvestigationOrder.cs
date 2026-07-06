using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class OpdInvestigationOrder : BaseEntity
{
    public Guid VisitId { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string TestOrStudyName { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Routine;
    public DateTimeOffset OrderedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? FulfilledReferenceId { get; set; }
}
