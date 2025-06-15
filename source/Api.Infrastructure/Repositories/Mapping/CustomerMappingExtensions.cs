using Api.Business.Entities;
using Api.Infrastructure.DbTables.OrganizationModels;

namespace Api.Infrastructure.Repositories.Mapping;

public static class CustomerMappingExtensions
{
    public static CustomerDomainModel ToDomainModel(this Customer entity)
    {
        return new CustomerDomainModel
        {
            CustomerId = entity.CustomerId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            Notes = entity.Notes,
            OrganizationId = entity.OrganizationId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Customer ToDbEntity(this CustomerDomainModel domain)
    {
        return new Customer
        {
            CustomerId = domain.CustomerId,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Email = domain.Email,
            PhoneNumber = domain.PhoneNumber,
            Notes = domain.Notes,
            OrganizationId = domain.OrganizationId,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static void UpdateFromDomainModel(this Customer entity, CustomerDomainModel domain)
    {
        entity.FirstName = domain.FirstName;
        entity.LastName = domain.LastName;
        entity.Email = domain.Email;
        entity.PhoneNumber = domain.PhoneNumber;
        entity.Notes = domain.Notes;
        entity.UpdatedAt = DateTime.UtcNow;
        // Note: We don't update CustomerId, OrganizationId, or CreatedAt
    }
}
