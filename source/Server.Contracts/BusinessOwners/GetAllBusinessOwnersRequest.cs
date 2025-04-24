using Server.Contracts.Client.Request;

namespace Server.Contracts.BusinessOwners;

public record GetAllBusinessOwnersRequest() : RequestBase(Route)
{
    public const string Route = "api/business-owners";
};