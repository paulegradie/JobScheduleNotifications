using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public class CreateScheduledJobDefinitionDto
{
    public Guid CustomerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AnchorDate { get; set; }
    public Frequency Frequency { get; set; }
    public int Interval { get; set; }
    public WeekDays[] WeekDays { get; set; }
    public int? DayOfMonth { get; set; }
    public string CronExpression { get; set; }
}