using System.Text.Json;
using Server.Client.Base;
using Server.Client.Exceptions;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Reminders;
using Server.Contracts.Endpoints.Reminders.Contracts;

namespace Server.Client.Endpoints;

internal sealed class InvoiceEndpoint : EndpointBase, IInvoiceEndpoint
{
    public InvoiceEndpoint(HttpClient client) : base(client) { }

    public async Task<OperationResult<InvoiceSentResponse>> SendInvoice(
        SendInvoiceRequest request,
        CancellationToken ct)
    {
        var uri = new Uri(request.GetApiRoute().ToString());

        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(request.PdfStream);

        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        form.Add(fileContent, "file", request.FileName);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
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
            return OperationResult<InvoiceSentResponse>.Failure(
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
            catch { }

            return OperationResult<InvoiceSentResponse>.Failure(errorMsg, response.StatusCode);
        }

        InvoiceSentResponse? value = null;
        try
        {
            value = JsonSerializer.Deserialize<InvoiceSentResponse>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            throw new ResponseDeserializationException(
                $"Failed to parse response from {uri}",
                ex
            );
        }

        return OperationResult<InvoiceSentResponse>.Success(value!, response.StatusCode);
    }
}