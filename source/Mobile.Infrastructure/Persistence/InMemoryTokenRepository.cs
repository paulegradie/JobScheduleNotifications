using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Mobile.Infrastructure.Persistence;

public sealed class InMemoryTokenRepository : ITokenRepository
{
    private string? _token;
    private TokenInfo? _tokenInfo;

    public Task StoreTokenAsync(string token)
    {
        _token = token;
        return Task.CompletedTask;
    }

    public string? RetrieveToken() => _token;

    public Task ClearTokenAsync()
    {
        _token = null;
        return Task.CompletedTask;
    }

    public Task StoreTokenMeta(TokenInfo tokenInfo)
    {
        _tokenInfo = tokenInfo;
        return Task.CompletedTask;
    }

    public Task<TokenInfo?> RetrieveTokenMeta()
    {
        return Task.FromResult(_tokenInfo);
    }
}