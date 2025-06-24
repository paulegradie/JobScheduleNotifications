using Api.Business.Repositories;
using Api.Controllers.Base;
using Api.Infrastructure.Identity;
using Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Endpoints.OrganizationSettings.Contracts;

namespace Api.Controllers;

[Authorize]
public class OrganizationSettingsController : BaseApiController
{
    private readonly IOrganizationSettingsRepository _settingsRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public OrganizationSettingsController(
        IOrganizationSettingsRepository settingsRepository,
        ICurrentUserContext currentUserContext)
    {
        _settingsRepository = settingsRepository;
        _currentUserContext = currentUserContext;
    }

    [HttpGet(GetOrganizationSettingsRequest.Route)]
    [Authorize(Policy = PolicyNames.ManageOrganizationSettings)]
    public async Task<ActionResult<GetOrganizationSettingsResponse>> GetSettings()
    {
        var organizationId = _currentUserContext.OrganizationId
            ?? throw new InvalidOperationException("User not assigned to an organization");

        var settings = await _settingsRepository.GetOrCreateAsync(organizationId);
        var dto = settings.ToDto();

        return Ok(new GetOrganizationSettingsResponse(dto));
    }

    [HttpPut(UpdateOrganizationSettingsRequest.Route)]
    [Authorize(Policy = PolicyNames.ManageOrganizationSettings)]
    public async Task<ActionResult<UpdateOrganizationSettingsResponse>> UpdateSettings(
        UpdateOrganizationSettingsRequest request)
    {
        var organizationId = _currentUserContext.OrganizationId
            ?? throw new InvalidOperationException("User not assigned to an organization");

        var settings = await _settingsRepository.GetOrCreateAsync(organizationId);
        
        // Update all fields
        settings.UpdateBusinessInfo(request.BusinessName, request.BusinessDescription, request.BusinessIdNumber);
        settings.UpdateContactInfo(request.Email, request.PhoneNumber);
        settings.UpdateAddress(request.StreetAddress, request.City, request.State, request.PostalCode, request.Country);
        settings.UpdateBankingInfo(request.BankName, request.BankBsb, request.BankAccountNumber, request.BankAccountName);

        var updatedSettings = await _settingsRepository.UpdateAsync(settings);
        var dto = updatedSettings.ToDto();

        return Ok(new UpdateOrganizationSettingsResponse(dto));
    }
}
