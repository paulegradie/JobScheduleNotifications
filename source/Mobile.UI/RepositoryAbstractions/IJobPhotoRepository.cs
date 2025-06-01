using Api.ValueTypes;

namespace Mobile.UI.RepositoryAbstractions;

public interface IJobPhotoRepository
{
    Task<bool> UploadPhotoAsync(string imagePath, CustomerId customerId, ScheduledJobDefinitionId jobDefinitionId, JobOccurrenceId jobOccurrenceId);
}