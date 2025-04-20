using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Customers;

public sealed record GetCustomersRequest() : RequestBase("api/customer");

public sealed record CustomersResponse(string Hello);