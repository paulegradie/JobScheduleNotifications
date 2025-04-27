using Server.Contracts.Common.Request;
using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.Customers.Contracts;

public sealed record GetCustomerByIdResponse(CustomerDto Customer) : ResponseBase;