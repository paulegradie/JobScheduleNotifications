using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.ScheduledJobs.Contracts;

public sealed record UpdateScheduledJobDefinitionResponse(ScheduledJobDefinitionDto ScheduledJobDefinitionDto);