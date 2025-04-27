using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.ScheduledJobs.Contracts;

public record ListJobDefinitionsByCustomerIdResponse(IEnumerable<ScheduledJobDefinitionDto> ScheduledJobDefinitions);