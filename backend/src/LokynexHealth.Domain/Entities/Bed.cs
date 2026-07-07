using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class Bed : BaseEntity
{
    public Guid WardId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public string? BedCategory { get; set; }
    public BedStatus Status { get; set; } = BedStatus.Available;
    public decimal? DailyRate { get; set; }
}