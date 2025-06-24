using Server.Client.Base;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.OrganizationSettings;
using Server.Contracts.Endpoints.OrganizationSettings.Contracts;

namespace Server.Client.Endpoints;

internal class OrganizationSettingsEndpoint : EndpointBase, IOrganizationSettingsEndpoint
{
    public OrganizationSettingsEndpoint(HttpClient client) : base(client)
    {
    }

    public Task<OperationResult<GetOrganizationSettingsResponse>> GetOrganizationSettingsAsync(
        GetOrganizationSettingsRequest request, 
        CancellationToken ct)
    {
        return GetAsync<GetOrganizationSettingsRequest, GetOrganizationSettingsResponse>(request, ct);
    }

    public Task<OperationResult<UpdateOrganizationSettingsResponse>> UpdateOrganizationSettingsAsync(
        UpdateOrganizationSettingsRequest request, 
        CancellationToken ct)
    {
        return PutAsync<UpdateOrganizationSettingsRequest, UpdateOrganizationSettingsResponse>(request, ct);
    }
}
