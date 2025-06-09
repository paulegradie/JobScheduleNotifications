using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Server.Contracts.Endpoints.JobPhotos;

public interface IDashboardEndpoint
{
    Task<OperationResult<DashboardResponse>> GetDashboardContent(
        GetDashboardContentRequest request,
        CancellationToken ct);
}