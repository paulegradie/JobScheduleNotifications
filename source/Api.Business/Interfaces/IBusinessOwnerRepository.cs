using JobScheduleNotifications.Core.Entities;

namespace JobScheduleNotifications.Core.Interfaces
{
    public interface IBusinessOwnerRepository
    {
        Task<BusinessOwnerDomainModel?> GetByIdAsync(Guid id);
        Task<BusinessOwnerDomainModel?> GetByEmailAsync(string email);
        Task<BusinessOwnerDomainModel> CreateAsync(BusinessOwnerDomainModel businessOwner);
        Task<BusinessOwnerDomainModel> UpdateAsync(BusinessOwnerDomainModel businessOwner);
        Task<bool> EmailExistsAsync(string email);
    }
} 