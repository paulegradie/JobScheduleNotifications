using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record ScheduledJobDefinitionDto(
    ScheduledJobDefinitionId Id,
    string Title,
    string Description,
    DateTime AnchorDate);