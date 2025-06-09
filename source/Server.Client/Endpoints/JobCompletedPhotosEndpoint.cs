using System.Text.Json;
using Server.Client.Base;
using Server.Client.Exceptions;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobPhotos;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Server.Client.Endpoints;

internal sealed class JobCompletedPhotosEndpoint : EndpointBase, IJobCompletedPhotosEndpoint
{
    public JobCompletedPhotosEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<OperationResult<JobCompletedPhotoUploadResponse>> Upload(
        UploadJobCompletedPhotoRequest request,
        CancellationToken ct)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(request.PhotoStream);

        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // Adjust as needed
        form.Add(fileContent, "file", request.FileName);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.ApiRoute);
        httpRequest.Content = form;

        HttpResponseMessage response;
        try
        {
            response = await Client.SendAsync(httpRequest, ct);
        }
        catch (Exception ex)
        {
            return OperationResult<JobCompletedPhotoUploadResponse>.Failure(
                ex.Message,
                System.Net.HttpStatusCode.InternalServerError
            );
        }

        var raw = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = raw;
            try
            {
                var err = JsonSerializer.Deserialize<Exceptions.ErrorResponse>(raw);
                if (err?.Messages?.Any() == true)
                    errorMsg = string.Join(", ", err.Messages);
            }
            catch
            {
                // ignore
            }

            return OperationResult<JobCompletedPhotoUploadResponse>.Failure(errorMsg, response.StatusCode);
        }

        JobCompletedPhotoUploadResponse? value;
        try
        {
            value = JsonSerializer.Deserialize<JobCompletedPhotoUploadResponse>(raw, new JsonSerializerOptions
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

        return OperationResult<JobCompletedPhotoUploadResponse>.Success(value!, response.StatusCode);
    }

    public async Task<OperationResult<JobCompletedPhotoUploadResponse>> Delete(DeleteJobCompletedPhotoRequest request, CancellationToken ct)
    {
        return await PutAsync<DeleteJobCompletedPhotoRequest, JobCompletedPhotoUploadResponse>(request, ct);
    }
}