using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record GetCustomersByBusinessNameRequest(string BusinessName) : RequestBase(Route)
{
    public const string Route = "api/customers/business-name/{businessName}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam("businessName", BusinessName);
        return route;
    }
}