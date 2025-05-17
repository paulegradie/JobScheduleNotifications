using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record ScheduledJobDefinitionDto(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    DateTime AnchorDate,
    string CronExpression,
    List<JobOccurrenceDto> JobOccurrences,
    string Title,
    string Description,
    int? DayOfMonth);