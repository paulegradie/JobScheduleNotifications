using Api.Business.Entities;
using Api.ValueTypes;

namespace Api.Business.Repositories;

public interface IScheduledJobDefinitionRepository
{
    Task<ScheduledJobDefinitionDomainModel?> GetAsync(ScheduledJobDefinitionId id);

    Task<List<ScheduledJobDefinitionDomainModel>> ListByCustomerAsync(CustomerId customerId);
    Task<List<ScheduledJobDefinitionDomainModel>> ListAllAsync();
    Task AddAsync(ScheduledJobDefinitionDomainModel def);
    Task<bool> UpdateAsync(ScheduledJobDefinitionDomainModel def);
}