using Server.Client.Base;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobPhotos;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Server.Client.Endpoints;

internal sealed class DashboardEndpoint : EndpointBase, IDashboardEndpoint
{
    public DashboardEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<OperationResult<DashboardResponse>> GetDashboardContent(
        GetDashboardContentRequest request,
        CancellationToken ct) =>
        await GetAsync<GetDashboardContentRequest, DashboardResponse>(request, ct);
}