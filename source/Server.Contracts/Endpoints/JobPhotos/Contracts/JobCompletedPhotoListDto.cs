namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed record JobCompletedPhotoListDto(List<JobCompletedPhotoDetails> JobCompletedPhotos);

public sealed record JobCompletedPhotoDetails(string Uri);