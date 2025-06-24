using Api.Business.Entities;
using Api.ValueTypes;

namespace Api.Business.Repositories;

public interface IOrganizationSettingsRepository
{
    /// <summary>
    /// Gets organization settings by organization ID
    /// </summary>
    Task<OrganizationSettingsDomainModel?> GetByOrganizationIdAsync(OrganizationId organizationId);
    
    /// <summary>
    /// Creates new organization settings
    /// </summary>
    Task<OrganizationSettingsDomainModel> CreateAsync(OrganizationSettingsDomainModel settings);
    
    /// <summary>
    /// Updates existing organization settings
    /// </summary>
    Task<OrganizationSettingsDomainModel> UpdateAsync(OrganizationSettingsDomainModel settings);
    
    /// <summary>
    /// Gets or creates organization settings for the given organization
    /// </summary>
    Task<OrganizationSettingsDomainModel> GetOrCreateAsync(OrganizationId organizationId);
}
