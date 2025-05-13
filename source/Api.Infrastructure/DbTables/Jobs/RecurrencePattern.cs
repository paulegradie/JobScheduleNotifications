using Api.Business.Entities.Base;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Server.Contracts.Dtos;

namespace Api.Infrastructure.DbTables.Jobs;

public class RecurrencePattern : IConvertToDto<RecurrencePatternDto>
{
    public RecurrencePattern()
    {
    }

    public RecurrencePattern(
        Frequency frequency,
        int interval,
        WeekDay[] weekDays,
        int? dayOfMonth = null,
        string? cronExpression = null)
    {
        Frequency = frequency;
        Interval = interval;
        WeekDays = weekDays;
        DayOfMonth = dayOfMonth;
        CronExpression = cronExpression;
    }

    public RecurrencePatternId RecurrencePatternId { get; set; }
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    public virtual ScheduledJobDefinition ScheduledJobDefinition { get; set; }
    public Frequency Frequency { get; set; } = Frequency.Weekly;
    public int Interval { get; set; } = 1;
    public WeekDay[] WeekDays { get; set; } = [ValueTypes.Enums.WeekDay.Monday];

    public int? DayOfMonth { get; set; } = 1;
    public string? CronExpression { get; set; } = string.Empty;

    public RecurrencePatternDto ToDto()
    {
        return new RecurrencePatternDto
        {
            Id = RecurrencePatternId,
            CronExpression = CronExpression,
        };
    }
}