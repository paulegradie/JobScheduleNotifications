using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public class RecurrencePatternDto
{
    public RecurrencePatternId Id { get; set; }
    public string? CronExpression { get; set; }
}