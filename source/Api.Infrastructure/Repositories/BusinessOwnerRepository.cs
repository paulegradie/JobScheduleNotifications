using Api.Infrastructure.Data;
using JobScheduleNotifications.Core.Entities;
using JobScheduleNotifications.Core.Interfaces;

namespace Api.Infrastructure.Repositories;

public class BusinessOwnerRepository : IBusinessOwnerRepository
{
    private readonly AppDbContext _dbContext;

    public BusinessOwnerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BusinessOwnerDomainModel?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessOwnerDomainModel?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessOwnerDomainModel> CreateAsync(BusinessOwnerDomainModel businessOwner)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessOwnerDomainModel> UpdateAsync(BusinessOwnerDomainModel businessOwner)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        throw new NotImplementedException();
    }
}