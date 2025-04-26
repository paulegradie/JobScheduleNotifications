using Server.Client.Base;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.JobOccurence;
using Server.Contracts.Client.Endpoints.JobOccurence.Contracts;
using Server.Contracts.Common;

namespace Server.Client.Endpoints;

/// <summary>
/// HTTP-based implementation of <see cref="IJobOccurrencesEndpoint"/>,
/// using the shared <see cref="EndpointBase"/> logic.
/// </summary>
internal class JobOccurrencesEndpoint : EndpointBase, IJobOccurrencesEndpoint
{
    public JobOccurrencesEndpoint(HttpClient client)
        : base(client)
    {
    }

    public Task<OperationResult<ListJobOccurrencesResponse>> ListAsync(
        ListJobOccurrencesRequest request,
        CancellationToken cancellationToken = default)
        => GetAsync<ListJobOccurrencesRequest, ListJobOccurrencesResponse>(
            request,
            cancellationToken);

    public Task<OperationResult<GetJobOccurrenceByIdResponse>> GetAsync(
        GetJobOccurrenceByIdRequest request,
        CancellationToken cancellationToken = default)
        => GetAsync<GetJobOccurrenceByIdRequest, GetJobOccurrenceByIdResponse>(
            request,
            cancellationToken);

    public Task<OperationResult<CreateJobOccurrenceResponse>> CreateAsync(
        CreateJobOccurrenceRequest request,
        CancellationToken cancellationToken = default)
        => PostAsync<CreateJobOccurrenceRequest, CreateJobOccurrenceResponse>(
            request,
            cancellationToken);

    public Task<OperationResult<UpdateJobOccurrenceResponse>> UpdateAsync(
        UpdateJobOccurrenceRequest request,
        CancellationToken cancellationToken = default)
        => PutAsync<UpdateJobOccurrenceRequest, UpdateJobOccurrenceResponse>(
            request,
            cancellationToken);

    public async Task<OperationResult<DeleteJobOccurrenceResponse>> DeleteAsync(
        DeleteJobOccurrenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await DeleteAsync<DeleteJobOccurrenceRequest, Unit>(
            request,
            cancellationToken);

        if (result.IsSuccess)
        {
            var resp = new DeleteJobOccurrenceResponse(
                OperationResult.Success(result.StatusCode));

            return OperationResult<DeleteJobOccurrenceResponse>.Success(
                resp,
                result.StatusCode);
        }

        return OperationResult<DeleteJobOccurrenceResponse>.Failure(
            result.ErrorMessage ?? string.Empty,
            result.StatusCode);
    }
}