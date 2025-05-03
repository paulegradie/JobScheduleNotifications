using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public class RecurrencePatternDto
{
    public RecurrencePatternId Id { get; set; }
    public Frequency Frequency { get; set; }
    public int Interval { get; set; }
    public WeekDay[] WeekDays { get; set; } = [];
    public int? DayOfMonth { get; set; }
    public string? CronExpression { get; set; }
}