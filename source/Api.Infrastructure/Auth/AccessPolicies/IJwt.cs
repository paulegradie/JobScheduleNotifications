using System.Security.Claims;
using Api.ValueTypes;

namespace Api.Infrastructure.Auth.AccessPolicies;

public interface IJwt
{
    string GenerateJwtToken(bool isAdmin, UserId userId, string userName);
    RefreshToken GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Expires { get; set; }
}
