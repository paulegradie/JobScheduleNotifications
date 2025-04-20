namespace Mobile.Infrastructure.Persistence;

public sealed class InMemoryTokenStore : ITokenStore
{
    private string? _token;
    public Task StoreTokenAsync(string token) { _token = token; return Task.CompletedTask; }
    public string? RetrieveToken() => _token;
    public Task ClearTokenAsync() { _token = null; return Task.CompletedTask; }
}