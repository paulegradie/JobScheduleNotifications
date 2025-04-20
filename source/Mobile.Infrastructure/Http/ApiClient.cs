using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Mobile.Core.Interfaces;

namespace Mobile.Infrastructure.Http;

public sealed class ApiClient : IApiClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly JsonSerializerOptions _jsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient client, IAuthService authService)
    {
        _httpClient = client;
        _httpClient.BaseAddress ??= new Uri("https://localhost:5001/api/");
        _authService = authService;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        await AttachBearerToken();
        using var resp = await _httpClient.GetAsync(endpoint);
        return await Deserialize<T>(resp);
    }

    public async Task<IEnumerable<T>?> GetListAsync<T>(string endpoint)
    {
        await AttachBearerToken();
        using var resp = await _httpClient.GetAsync(endpoint);
        return await Deserialize<IEnumerable<T>>(resp);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        await AttachBearerToken();
        using var content = new StringContent(
            JsonSerializer.Serialize(data, _jsonOptions),
            Encoding.UTF8,
            "application/json");
        using var resp = await _httpClient.PostAsync(endpoint, content);
        return await Deserialize<T>(resp);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        await AttachBearerToken();
        using var content = new StringContent(
            JsonSerializer.Serialize(data, _jsonOptions),
            Encoding.UTF8,
            "application/json");
        using var resp = await _httpClient.PutAsync(endpoint, content);
        return await Deserialize<T>(resp);
    }

    public async Task DeleteAsync(string endpoint)
    {
        await AttachBearerToken();
        using var resp = await _httpClient.DeleteAsync(endpoint);
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
    }

    private async Task AttachBearerToken()
    {
        var token = _authService.GetToken();
        if (!string.IsNullOrWhiteSpace(token))
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<T?> Deserialize<T>(HttpResponseMessage resp)
    {
        var json = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"{resp.StatusCode}: {json}");
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public void Dispose() => _httpClient.Dispose();
}
