using Api.Business.Entities.Base;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class RecurrencePatternDomainModel : DomainModelBase<RecurrencePatternDto>
{
    public RecurrencePatternDomainModel()
    {
    }

    public RecurrencePatternDomainModel(Frequency frequency, int interval, WeekDay[] weekDays, int? dayOfMonth = null, string? cronExpression = null)
    {
        Frequency = frequency;
        Interval = interval;
        WeekDays = weekDays;
        DayOfMonth = dayOfMonth;
        CronExpression = cronExpression;
    }

    public RecurrencePatternDomainModel(RecurrencePatternDto dto)
    {
        Id = dto.Id;
        Frequency = dto.Frequency;
        Interval = dto.Interval;
        WeekDays = dto.WeekDays;
        DayOfMonth = dto.DayOfMonth;
        CronExpression = dto.CronExpression;
    }

    // Unique identifier
    public RecurrencePatternId Id { get; set; }

    public Frequency Frequency { get; set; } = Frequency.Weekly;
    public int Interval { get; set; } = 1;
    public WeekDay[] WeekDays { get; set; } = [];
    public int? DayOfMonth { get; set; }
    public string? CronExpression { get; set; }

    public override RecurrencePatternDto ToDto()
    {
        return new RecurrencePatternDto
        {
            Id = Id,
            Frequency = Frequency,
            Interval = Interval,
            WeekDays = WeekDays,
            DayOfMonth = DayOfMonth,
            CronExpression = CronExpression
        };
    }
}