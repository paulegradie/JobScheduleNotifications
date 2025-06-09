namespace Server.Contracts.Endpoints.Auth.Contracts;

public class TokenInfo
{
    public TokenInfo()
    {
    }

    public TokenInfo(string email, string accessToken, DateTimeOffset expires)
    {
        AccessToken = accessToken;
        Email = email;
        ExpiresAt = expires.DateTime;
    }

    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Email { get; set; }
    public DateTime ExpiresAt { get; set; }
}