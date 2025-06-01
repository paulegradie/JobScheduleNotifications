using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Customers.Contracts;

public sealed record GetCustomerByIdRequest(CustomerId Id) : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, Id.ToString());
        return route;
    }
}