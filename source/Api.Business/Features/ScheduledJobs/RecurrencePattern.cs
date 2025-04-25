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
        WeekDays daysOfWeek,
        int? dayOfMonth = null,
        string? cronExpression = null)
    {
        Frequency = frequency;
        Interval = interval;
        DaysOfWeek = daysOfWeek;
        DayOfMonth = dayOfMonth;
        CronExpression = cronExpression;
    }

    public RecurrencePatternId Id { get; set; }

    public Frequency Frequency { get; set; } = Frequency.Weekly;
    public int Interval { get; set; } = 1;
    public WeekDays DaysOfWeek { get; set; } = WeekDays.Monday;

    public int? DayOfMonth { get; set; }
    public string? CronExpression { get; set; }
}