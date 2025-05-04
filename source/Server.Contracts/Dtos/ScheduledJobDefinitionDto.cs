using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record ScheduledJobDefinitionDto(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    DateTime AnchorDate,
    RecurrencePatternDto Pattern,
    List<JobOccurrenceDto> JobOccurrences,
    string Title,
    string Description);