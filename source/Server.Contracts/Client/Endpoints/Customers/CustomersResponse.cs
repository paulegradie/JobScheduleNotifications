using Server.Contracts.Customers;

namespace Server.Contracts.Client.Endpoints.Customers;

public sealed record CustomersResponse(List<CustomerDto> Customers);