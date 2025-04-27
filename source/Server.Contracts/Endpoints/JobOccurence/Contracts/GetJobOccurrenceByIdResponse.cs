using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.JobOccurence.Contracts;

public sealed record GetJobOccurrenceByIdResponse(JobOccurrenceDto JobOccurrence);