using System.Net;
using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Mobile.Infrastructure.Repositories;

public class JobCompletedPhotoRepository : IJobCompletedPhotoRepository
{
    private readonly IServerClient _serverClient;

    public JobCompletedPhotoRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<OperationResult<JobCompletedPhotoUploadResponse>> UploadPhotoAsync(string imagePath, CustomerId customerId, ScheduledJobDefinitionId jobDefinitionId,
        JobOccurrenceId jobOccurrenceId)
    {
        if (!File.Exists(imagePath))
        {
            return OperationResult<JobCompletedPhotoUploadResponse>
                .Failure("Failed to find photo locally", HttpStatusCode.Ambiguous);
        }

        await using var stream = File.OpenRead(imagePath);
        var request = new UploadJobCompletedPhotoRequest(customerId, jobDefinitionId, jobOccurrenceId, stream, Path.GetFileName(imagePath));

        var result = await _serverClient.JobCompletedPhotos.Upload(request, CancellationToken.None);
        return result;
    }

    public async Task<OperationResult<JobCompletedPhotoDeleteResponse>> DeletePhotoAsync(CustomerId customerId, ScheduledJobDefinitionId jobDefinitionId, JobOccurrenceId jobOccurrenceId,
        JobCompletedPhotoId jobCompletedPhotoId)
    {
        var request = new DeleteJobCompletedPhotoRequest(customerId, jobDefinitionId, jobOccurrenceId, jobCompletedPhotoId);
        var response = await _serverClient.JobCompletedPhotos.Delete(request, CancellationToken.None);
        return response;
    }

    public Task<OperationResult<JobCompletedPhotoListResponse>> ListPhotoAsync(CustomerId customerId, ScheduledJobDefinitionId jobDefinitionId, JobOccurrenceId jobOccurrenceId)
    {
        var request = new ListJobCompletedPhotosRequest(customerId, jobDefinitionId, jobOccurrenceId);
        return _serverClient.JobCompletedPhotos.List(request, CancellationToken.None);
    }
}