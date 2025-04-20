using System.Text.Json;
using System.Text.Json.Serialization;
using Server.Client.Base;
using Server.Contracts.Client.Endpoints.Customers;

namespace Server.Client.Endpoints;

internal class CustomersEndpoint : EndpointBase, ICustomersEndpoint
{
    public CustomersEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<CustomersResponse> GetCustomersAsync(
        GetCustomersRequest request,
        CancellationToken cancellationToken)
    {
        using var httpResponse = await Client.GetAsync(
            request.GetApiRoute().ToString(),
            cancellationToken
        );
        httpResponse.EnsureSuccessStatusCode();

        await using var contentStream = await httpResponse
            .Content
            .ReadAsStreamAsync(cancellationToken);

        var result = await JsonSerializer.DeserializeAsync<CustomersResponse>(
            contentStream,
            options: new JsonSerializerOptions
            {
                AllowOutOfOrderMetadataProperties = false,
                AllowTrailingCommas = false,
                DefaultBufferSize = 0,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                DictionaryKeyPolicy = null,
                Encoder = null,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = false,
                MaxDepth = 0,
                NewLine = null,
                NumberHandling = JsonNumberHandling.Strict,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null,
                ReadCommentHandling = JsonCommentHandling.Disallow,
                ReferenceHandler = null,
                RespectNullableAnnotations = false,
                RespectRequiredConstructorParameters = false,
                TypeInfoResolver = null,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
                WriteIndented = false,
                IndentCharacter = '\0',
                IndentSize = 0
            },
            cancellationToken
        );

        if (result is null)
            throw new InvalidOperationException(
                "Failed to deserialize CustomersResponse");

        return result;
    }
}