using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public class RecurrencePatternDto
{
    public RecurrencePatternId Id { get; set; }
    public string? CronExpression { get; set; }
}