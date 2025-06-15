using System.Text.Json;
using Server.Client.Base;
using Server.Client.Exceptions;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Invoices;
using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Server.Client.Endpoints;

internal sealed class InvoiceEndpoint : EndpointBase, IInvoiceEndpoint
{
    public InvoiceEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<OperationResult<InvoiceSavedResponse>> SaveInvoice(SaveInvoiceRequest request, CancellationToken ct)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(request.InvoiceStream);

        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        form.Add(fileContent, "file", request.FileName);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.ApiRoute)
        {
            Content = form
        };

        HttpResponseMessage response;
        try
        {
            response = await Client.SendAsync(httpRequest, ct);
        }
        catch (Exception ex)
        {
            return OperationResult<InvoiceSavedResponse>.Failure(
                ex.Message,
                System.Net.HttpStatusCode.InternalServerError
            );
        }

        var raw = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            string errorMsg = raw;
            try
            {
                var err = JsonSerializer.Deserialize<Exceptions.ErrorResponse>(raw);
                if (err?.Messages?.Any() == true)
                    errorMsg = string.Join(", ", err.Messages);
            }
            catch
            {
            }

            return OperationResult<InvoiceSavedResponse>.Failure(errorMsg, response.StatusCode);
        }

        InvoiceSavedResponse? value = null;
        try
        {
            value = JsonSerializer.Deserialize<InvoiceSavedResponse>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            throw new ResponseDeserializationException(
                $"Failed to parse response from {request.ApiRoute}",
                ex
            );
        }

        return OperationResult<InvoiceSavedResponse>.Success(value!, response.StatusCode);
    }

    public async Task<OperationResult<InvoiceSentResponse>> SendInvoice(SendInvoiceRequest request, CancellationToken cancellationToken)
    {
        return await PostAsync<SendInvoiceRequest, InvoiceSentResponse>(request, cancellationToken);
    }
}