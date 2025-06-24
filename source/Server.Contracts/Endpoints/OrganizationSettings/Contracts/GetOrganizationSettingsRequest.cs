using Server.Contracts.Common.Request;
using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.OrganizationSettings.Contracts;

public sealed record GetOrganizationSettingsRequest() : RequestBase(Route)
{
    public const string Route = "api/organization/settings";
}

public sealed record GetOrganizationSettingsResponse(OrganizationSettingsDto Settings);
