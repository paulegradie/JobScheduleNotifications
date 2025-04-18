using JobScheduleNotifications.Core.Entities;

namespace JobScheduleNotifications.Core.Interfaces
{
    public interface IBusinessOwnerRepository
    {
        Task<BusinessOwner?> GetByIdAsync(Guid id);
        Task<BusinessOwner?> GetByEmailAsync(string email);
        Task<BusinessOwner> CreateAsync(BusinessOwner businessOwner);
        Task<BusinessOwner> UpdateAsync(BusinessOwner businessOwner);
        Task<bool> EmailExistsAsync(string email);
    }
} 