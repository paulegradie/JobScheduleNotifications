using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record GetCustomerByIdResponse(CustomerDto Customer) : ResponseBase;