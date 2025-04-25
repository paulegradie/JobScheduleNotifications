using Server.Contracts.Dtos;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs.Contracts;

public sealed record UpdateScheduledJobDefinitionResponse(ScheduledJobDefinitionDto ScheduledJobDefinitionDto);