using Api.ValueTypes;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Server.Contracts.Dtos;

namespace Api.Business.Services;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request, IdentityUserId currentUserId);
}