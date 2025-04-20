using Mobile.Core.Interfaces;
using Mobile.Core.Models;
using Mobile.Core.Services;
using Mobile.Infrastructure.Persistence;
using IAuthService = Mobile.Core.Interfaces.IAuthService;

namespace Mobile.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IApiClient _api;
    private readonly ITokenStore _store;

    public AuthService(IApiClient api, ITokenStore store)
    {
        _api = api;
        _store = store;
    }

    public async Task RegisterAsync(RegisterRequest req)
    {
        var resp = await _api.PostAsync<AuthResponse>("auth/register", req)
                   ?? throw new InvalidOperationException("Registration failed");
        await _store.StoreTokenAsync(resp.Token);
    }

    public async Task LoginAsync(LoginRequest req)
    {
        var resp = await _api.PostAsync<AuthResponse>("auth/login", req)
                   ?? throw new InvalidOperationException("Login failed");
        await _store.StoreTokenAsync(resp.Token!);
    }

    public string? GetToken() => _store.RetrieveToken();

    public Task LogoutAsync() => _store.ClearTokenAsync();
}