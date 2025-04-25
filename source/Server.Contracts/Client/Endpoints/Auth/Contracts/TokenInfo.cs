namespace Server.Contracts.Client.Endpoints.Auth.Contracts;

public class TokenInfo
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Email { get; set; }
    public DateTime ExpiresAt { get; set; }
}