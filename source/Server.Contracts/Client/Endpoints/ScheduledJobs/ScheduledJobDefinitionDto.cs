using Api.ValueTypes;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

public record ScheduledJobDefinitionDto(
    ScheduledJobDefinitionId Id,
    string Title,
    string Description,
    DateTime AnchorDate);