using Server.Contracts.Dtos;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs.Contracts;

public record ListJobDefinitionsByCustomerIdResponse(IEnumerable<ScheduledJobDefinitionDto> ScheduledJobDefinitions);