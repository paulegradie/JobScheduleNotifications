using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Mobile.Infrastructure.Repositories;

public class JobPhotoRepository : IJobPhotoRepository
{
    private readonly IServerClient _serverClient;

    public JobPhotoRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<bool> UploadPhotoAsync(string imagePath, CustomerId customerId, ScheduledJobDefinitionId jobDefinitionId, JobOccurrenceId jobOccurrenceId)
    {
        if (!File.Exists(imagePath)) return false;

        await using var stream = File.OpenRead(imagePath);
        var request = new UploadJobPhotoRequest(customerId, jobDefinitionId, jobOccurrenceId, stream, Path.GetFileName(imagePath));

        var result = await _serverClient.JobPhotos.Upload(request, CancellationToken.None);
        return result.IsSuccess;
    }
}