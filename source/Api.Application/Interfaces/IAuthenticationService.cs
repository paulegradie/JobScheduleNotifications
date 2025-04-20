using JobScheduleNotifications.Core.Entities;

namespace JobScheduleNotifications.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<(BusinessOwner Owner, string Token)> RegisterBusinessOwnerAsync(
            string email,
            string password,
            string businessName,
            string firstName,
            string lastName,
            string phoneNumber,
            string businessAddress,
            string businessDescription);

        Task<(BusinessOwner Owner, string Token)> LoginAsync(string email, string password);
        
        string GenerateJwtToken(BusinessOwner businessOwner);
        
        Task<bool> ValidateTokenAsync(string token);
    }
} 