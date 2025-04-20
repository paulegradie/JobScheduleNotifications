using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Mobile.Core.Services;

public class HttpClientService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpClientService(IAuthService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7001/api/");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        await AddAuthorizationHeaderAsync();
        var response = await _httpClient.GetAsync(new Uri(endpoint));
        return await HandleResponseAsync<T>(response);
    }

    public async Task<IEnumerable<T>?> GetListAsync<T>(string endpoint)
    {
        await AddAuthorizationHeaderAsync();
        var response = await _httpClient.GetAsync(new Uri(endpoint));
        return await HandleResponseAsync<IEnumerable<T>>(response);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        await AddAuthorizationHeaderAsync();
        using var content = new StringContent(
            JsonSerializer.Serialize(data, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(new Uri(endpoint), content);
        return await HandleResponseAsync<T>(response);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        await AddAuthorizationHeaderAsync();
        var content = new StringContent(
            JsonSerializer.Serialize(data, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync(new Uri(endpoint), content);
        return await HandleResponseAsync<T>(response);
    }

    public async Task DeleteAsync(string endpoint)
    {
        await AddAuthorizationHeaderAsync();
        var response = await _httpClient.DeleteAsync(new Uri(endpoint));
        await HandleResponseAsync<object>(response);
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await _authService.GetAuthTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"API request failed: {response.StatusCode} - {errorContent}");
    }

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
} 