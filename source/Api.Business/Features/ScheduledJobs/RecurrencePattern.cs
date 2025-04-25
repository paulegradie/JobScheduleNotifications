using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Api.Business.Features.ScheduledJobs;

public class RecurrencePattern
{
    // parameterless ctor still needed for e.g. serializers/tests
    public RecurrencePattern()
    {
    }

    public RecurrencePattern(
        Frequency frequency,
        int interval,
        WeekDays[] weekDays,
        int? dayOfMonth = null,
        string? cronExpression = null)
    {
        Frequency = frequency;
        Interval = interval;
        WeekDays = weekDays;
        DayOfMonth = dayOfMonth;
        CronExpression = cronExpression;
    }

    public RecurrencePatternId Id { get; set; }

    public Frequency Frequency { get; set; } = Frequency.Weekly;
    public int Interval { get; set; } = 1;
    public WeekDays[] WeekDays { get; set; } = [ValueTypes.Enums.WeekDays.Monday];

    public int? DayOfMonth { get; set; }
    public string? CronExpression { get; set; }
}