using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record DeleteCustomerRequest(CustomerId Id) : RequestBase(Route)
{
    public const string Route = $"api/customers/{IdSegmentParam}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(IdSegmentParam, Id.ToString());
        return route;
    }
};