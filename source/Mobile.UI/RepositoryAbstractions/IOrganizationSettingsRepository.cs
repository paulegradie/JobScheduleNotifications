using Server.Contracts.Dtos;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.OrganizationSettings.Contracts;

namespace Mobile.UI.RepositoryAbstractions;

public interface IOrganizationSettingsRepository
{
    Task<OperationResult<OrganizationSettingsDto>> GetOrganizationSettingsAsync(CancellationToken ct = default);
    Task<OperationResult<OrganizationSettingsDto>> UpdateOrganizationSettingsAsync(UpdateOrganizationSettingsRequest request, CancellationToken ct = default);
}
