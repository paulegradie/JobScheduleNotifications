using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record GetCustomersRequest() : RequestBase(Route)
{
    public const string Route = "api/customers";
};