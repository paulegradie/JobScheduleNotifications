using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.OrganizationSettings.Contracts;

namespace Mobile.Infrastructure.Repositories;

internal class OrganizationSettingsRepository : IOrganizationSettingsRepository
{
    private readonly IServerClient _serverClient;

    public OrganizationSettingsRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<OperationResult<OrganizationSettingsDto>> GetOrganizationSettingsAsync(CancellationToken ct = default)
    {
        var request = new GetOrganizationSettingsRequest();
        var result = await _serverClient.OrganizationSettings.GetOrganizationSettingsAsync(request, ct);

        if (!result.IsSuccess)
            return OperationResult<OrganizationSettingsDto>
                .Failure(result.ErrorMessage ?? "Could not load organization settings", result.StatusCode);

        return OperationResult<OrganizationSettingsDto>
            .Success(result.Value.Settings, result.StatusCode);
    }

    public async Task<OperationResult<OrganizationSettingsDto>> UpdateOrganizationSettingsAsync(
        UpdateOrganizationSettingsRequest request, 
        CancellationToken ct = default)
    {
        var result = await _serverClient.OrganizationSettings.UpdateOrganizationSettingsAsync(request, ct);

        if (!result.IsSuccess)
            return OperationResult<OrganizationSettingsDto>
                .Failure(result.ErrorMessage ?? "Could not update organization settings", result.StatusCode);

        return OperationResult<OrganizationSettingsDto>
            .Success(result.Value.Settings, result.StatusCode);
    }
}
