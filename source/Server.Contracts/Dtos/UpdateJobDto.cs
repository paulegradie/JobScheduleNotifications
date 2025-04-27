using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public class UpdateJobDto
{
    public CustomerId CustomerId { get; set; }
    public ScheduledJobDefinitionId JobDefinitionId { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }

    public Frequency? Frequency { get; set; }
    public int? Interval { get; set; }
    public WeekDays[]? WeekDays { get; set; }
    public int? DayOfMonth { get; set; }
    public string? CronExpression { get; set; }
    public DateTime? AnchorDate { get; set; }
}