using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public record UpdateJobDto(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    string Title,
    string Description,
    string CronExpression,
    DateTime AnchorDate,
    int? DayOfMonth);