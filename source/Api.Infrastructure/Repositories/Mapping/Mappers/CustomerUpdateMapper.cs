using Api.Business.Repositories;
using Api.Business.Repositories.Internal;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;
using Server.Contracts.Client.Endpoints.Customers.Contracts;

namespace Api.Infrastructure.Repositories.Mapping.Mappers;

public class CustomerUpdateMapper : IMapFrom<UpdateCustomerRequest, Customer>
{
    private readonly ICrudRepository<Customer, CustomerId> _repository;

    public CustomerUpdateMapper(ICrudRepository<Customer, CustomerId> repository)
    {
        _repository = repository;
    }

    public async Task<Customer?> Map(UpdateCustomerRequest from)
    {
        var existingCustomer = await _repository.GetByIdAsync(from.Id);
        if (existingCustomer == null)
        {
            return null;
        }

        return existingCustomer
            .UpdateFirstName(from.FirstName)
            .UpdateLastName(from.LastName);
    }
}