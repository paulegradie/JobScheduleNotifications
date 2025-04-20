using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JobScheduleNotifications.Core.Entities;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JobScheduleNotifications.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IBusinessOwnerRepository _businessOwnerRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            IBusinessOwnerRepository businessOwnerRepository,
            IConfiguration configuration)
        {
            _businessOwnerRepository = businessOwnerRepository;
            _configuration = configuration;
        }

        public async Task<(BusinessOwner Owner, string Token)> RegisterBusinessOwnerAsync(
            string email,
            string password,
            string businessName,
            string firstName,
            string lastName,
            string phoneNumber,
            string businessAddress,
            string businessDescription)
        {
            if (await _businessOwnerRepository.EmailExistsAsync(email))
            {
                throw new InvalidOperationException("Email already registered");
            }

            var businessOwner = new BusinessOwner
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant(),
                PasswordHash = HashPassword(password),
                BusinessName = businessName,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                BusinessAddress = businessAddress,
                BusinessDescription = businessDescription,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _businessOwnerRepository.CreateAsync(businessOwner);
            var token = GenerateJwtToken(businessOwner);

            return (businessOwner, token);
        }

        public async Task<(BusinessOwner Owner, string Token)> LoginAsync(string email, string password)
        {
            var businessOwner = await _businessOwnerRepository.GetByEmailAsync(email.ToLowerInvariant());
            if (businessOwner == null)
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            if (!VerifyPassword(password, businessOwner.PasswordHash))
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            if (!businessOwner.IsActive)
            {
                throw new InvalidOperationException("Account is not active");
            }

            var token = GenerateJwtToken(businessOwner);
            return (businessOwner, token);
        }

        public string GenerateJwtToken(BusinessOwner businessOwner)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, businessOwner.Id.ToString()),
                    new Claim(ClaimTypes.Email, businessOwner.Email),
                    new Claim(ClaimTypes.Name, businessOwner.BusinessName),
                    new Claim("FirstName", businessOwner.FirstName),
                    new Claim("LastName", businessOwner.LastName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured"));
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
#pragma warning disable CA5404
                    ValidateIssuer = false,
#pragma warning restore CA5404
#pragma warning disable CA5404
                    ValidateAudience = false,
#pragma warning restore CA5404
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                var user = await _businessOwnerRepository.GetByIdAsync(userId);
                return user != null && user.IsActive;
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return false;
            }
        }

        private static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
} 