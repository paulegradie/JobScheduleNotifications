using Server.Contracts.Client.Request;
using Server.Contracts.Customers;

namespace Server.Contracts.Client.Endpoints.Customers;

public sealed record GetCustomersRequest() : RequestBase("api/customer");

public sealed record CustomersResponse(List<CustomerDto> Customers);