using Server.Contracts.Endpoints.JobPhotos.Contracts;
using Server.Contracts.Endpoints.Reminders.Contracts;

namespace Server.Contracts.Endpoints.JobPhotos;

public interface IJobPhotosEndpoint
{
    Task<OperationResult<JobPhotoUploadResponse>> Upload(UploadJobPhotoRequest request, CancellationToken ct);
}