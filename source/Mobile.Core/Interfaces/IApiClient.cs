namespace Mobile.Core.Interfaces;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<IEnumerable<T>?> GetListAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task DeleteAsync(string endpoint);
}