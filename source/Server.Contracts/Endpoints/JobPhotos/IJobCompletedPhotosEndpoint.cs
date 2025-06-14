using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Server.Contracts.Endpoints.JobPhotos;

public interface IJobCompletedPhotosEndpoint
{
    Task<OperationResult<JobCompletedPhotoUploadResponse>> Upload(UploadJobCompletedPhotoRequest request, CancellationToken ct);
    Task<OperationResult<JobCompletedPhotoDeleteResponse>> Delete(DeleteJobCompletedPhotoRequest request, CancellationToken ct);
    Task<OperationResult<JobCompletedPhotoListResponse>> List(ListJobCompletedPhotosRequest request, CancellationToken ct);
}