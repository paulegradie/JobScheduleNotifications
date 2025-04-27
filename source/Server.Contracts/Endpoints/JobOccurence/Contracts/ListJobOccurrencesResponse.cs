using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.JobOccurence.Contracts;

public sealed record ListJobOccurrencesResponse(IReadOnlyList<JobOccurrenceDto> Occurrences);