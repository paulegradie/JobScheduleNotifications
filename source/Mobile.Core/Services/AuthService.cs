using Server.Contracts.Authentication;

namespace Mobile.Core.Services;

public class AuthService : IAuthService
{
    private readonly HttpClientService _httpClient;
    private const string authTokenKey = "auth_token";
    private const string userEmailKey = "user_email";
    private const string userPasswordKey = "user_password";

    public AuthService(HttpClientService httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsync<LoginResponseDto>("auth/login", loginRequest);
            
            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                await SecureStorage.SetAsync(authTokenKey, response.Token);
                await SecureStorage.SetAsync(userEmailKey, email);
                await SecureStorage.SetAsync(userPasswordKey, password);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterBusinessOwnerDto registration)
    {
        try
        {
            var response = await _httpClient.PostAsync<RegisterResponseDto>("auth/register", registration);
            return response is { Success: true };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Registration Error: {ex.Message}");
            return false;
        }
    }

    public Task<bool> LogoutAsync()
    {
        try
        {
            SecureStorage.Remove(authTokenKey);
            SecureStorage.Remove(userEmailKey);
            SecureStorage.Remove(userPasswordKey);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Logout Error: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetAuthTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string> GetAuthTokenAsync()
    {
        return await SecureStorage.GetAsync(authTokenKey) ?? string.Empty;
    }
}

public class AuthResponse
{
    public string? Token { get; set; }
} 