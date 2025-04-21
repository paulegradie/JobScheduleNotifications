using System.Net.Http.Json;
using System.Text.Json;
using Server.Client.Base;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Customers;

namespace Server.Client.Endpoints;

internal class CustomersEndpoint : EndpointBase, ICustomersEndpoint
{
    public CustomersEndpoint(HttpClient client) : base(client) { }

    public async Task<CustomersResponse> GetCustomersAsync(
        GetCustomersRequest request,
        CancellationToken cancellationToken)
    {
        using var httpResponse = await Client.GetAsync(
            request.GetApiRoute().ToString(), cancellationToken);

        httpResponse.EnsureSuccessStatusCode();

        await using var contentStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<CustomersResponse>(contentStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken);

        return result ?? throw new InvalidOperationException("Failed to deserialize CustomersResponse");
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await Client.GetAsync($"api/customer/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<CustomerDto>(stream, cancellationToken: cancellationToken);
    }

    public async Task<CustomerDto?> CreateCustomerAsync(CreateCustomerDto customer, CancellationToken cancellationToken)
    {
        var content = JsonContent.Create(customer);
        var response = await Client.PostAsync("api/customer", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<CustomerDto>(stream, cancellationToken: cancellationToken);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto customer, CancellationToken cancellationToken)
    {
        var content = JsonContent.Create(customer);
        var response = await Client.PutAsync($"api/customer/{id}", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<CustomerDto>(stream, cancellationToken: cancellationToken);
    }

    public async Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await Client.DeleteAsync($"api/customer/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
