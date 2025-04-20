namespace Mobile.Infrastructure.Persistence;

/// <summary>Abstracts storage of the JWT on the client (in-memory, secure storage, etc.).</summary>
public interface ITokenStore
{
    Task StoreTokenAsync(string token);
    string? RetrieveToken();
    Task ClearTokenAsync();
}