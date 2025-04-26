using Server.Contracts.Dtos;

namespace Server.Contracts.Client.Endpoints.JobOccurence.Contracts;

public sealed record ListJobOccurrencesResponse(IReadOnlyList<JobOccurrenceDto> Occurrences);