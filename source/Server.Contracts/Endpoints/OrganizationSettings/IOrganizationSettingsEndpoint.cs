using Server.Contracts.Endpoints.OrganizationSettings.Contracts;

namespace Server.Contracts.Endpoints.OrganizationSettings;

public interface IOrganizationSettingsEndpoint
{
    Task<OperationResult<GetOrganizationSettingsResponse>> GetOrganizationSettingsAsync(
        GetOrganizationSettingsRequest request, 
        CancellationToken ct);
        
    Task<OperationResult<UpdateOrganizationSettingsResponse>> UpdateOrganizationSettingsAsync(
        UpdateOrganizationSettingsRequest request, 
        CancellationToken ct);
}
