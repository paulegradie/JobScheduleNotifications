using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record GetCustomerByIdRequest(Guid Id) : RequestBase(Route)
{
    public const string Route = "api/customers/{id}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam("id", Id.ToString());
        return route;
    }
}