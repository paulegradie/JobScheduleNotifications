using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Infrastructure.Auth.AccessPolicies;

public class Jwt : IJwt
{
    private readonly string _jwtKey;

    public Jwt(IOptions<AuthenticationOptions> authOptions)
    {
        _jwtKey = authOptions.Value.Key;
    }

    public string GenerateJwtToken(bool isAdmin, string userName)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, isAdmin ? UserRoles.AdminRole : UserRoles.MemberRole)
        };

        var token = new JwtSecurityToken(
            issuer: null, // how do I know if I need an issuer?
            audience: null, // same
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(7) // Set your preferred expiration time
        };
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        {
            // Your token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                // ... your existing validation parameters
                ValidateLifetime = false // Important: Don't validate lifetime for expired token verification
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}