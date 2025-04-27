using Api.ValueTypes;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Api.Business.Services;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request, IdentityUserId currentUserId);
}