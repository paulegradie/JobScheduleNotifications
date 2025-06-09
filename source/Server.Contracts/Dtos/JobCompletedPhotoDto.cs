using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record JobCompletedPhotoDto(
    JobOccurrenceId JobOccurrenceId,
    CustomerId CustomerId,
    JobCompletedPhotoId JobCompletedPhotoId,
    string PhotoUri
);