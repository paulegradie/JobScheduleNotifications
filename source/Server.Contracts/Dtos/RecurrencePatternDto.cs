using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public record RecurrencePatternDto()
{
    public RecurrencePatternId Id { get; set; }

    public Frequency Frequency { get; set; } = Frequency.Weekly;
    public int Interval { get; set; } = 1;
    public WeekDays[] WeekDays { get; set; } = [Api.ValueTypes.Enums.WeekDays.Monday];

    public int? DayOfMonth { get; set; }
    public string? CronExpression { get; set; }
}