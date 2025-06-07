using System.Threading.Tasks;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Mobile.UI.RepositoryAbstractions;

/// <summary>Abstracts storage of the JWT on the client (in-memory, secure storage, etc.).</summary>
public interface ITokenRepository
{
    Task StoreTokenAsync(string token);
    string? RetrieveToken();
    Task ClearTokenAsync();
    Task StoreTokenMeta(TokenInfo tokenInfo);
    Task<TokenInfo?> RetrieveTokenMeta();
}