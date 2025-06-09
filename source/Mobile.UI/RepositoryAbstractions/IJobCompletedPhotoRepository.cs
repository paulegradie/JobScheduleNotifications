using Api.ValueTypes;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Mobile.UI.RepositoryAbstractions;

public interface IJobCompletedPhotoRepository
{
    Task<OperationResult<JobCompletedPhotoUploadResponse>> UploadPhotoAsync(
        string imagePath,
        CustomerId customerId,
        ScheduledJobDefinitionId jobDefinitionId,
        JobOccurrenceId jobOccurrenceId);

    Task<OperationResult<JobCompletedPhotoDeleteResponse>> DeletePhotoAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId jobDefinitionId,
        JobOccurrenceId jobOccurrenceId,
        JobCompletedPhotoId jobCompletedPhotoId);
}