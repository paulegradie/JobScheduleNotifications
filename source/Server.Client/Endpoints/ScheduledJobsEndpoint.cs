using Server.Client.Base;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.ScheduledJobs;
using Server.Contracts.Endpoints.ScheduledJobs.Contracts;

namespace Server.Client.Endpoints;

/// <summary>
/// HTTP-based implementation of <see cref="IScheduledJobsEndpoint"/>,
/// using the shared <see cref="EndpointBase"/> logic.
/// </summary>
internal class ScheduledJobsEndpoint : EndpointBase, IScheduledJobsEndpoint
{
    public ScheduledJobsEndpoint(HttpClient client)
        : base(client)
    {
    }

    public Task<OperationResult<ListJobDefinitionsByCustomerIdResponse>> ListJobDefinitionsAsync(
        ListJobDefinitionsByCustomerIdRequest request,
        CancellationToken ct)
        => GetAsync<ListJobDefinitionsByCustomerIdRequest, ListJobDefinitionsByCustomerIdResponse>(request, ct);

    public Task<OperationResult<GetScheduledJobDefinitionByIdResponse>> GetScheduledJobDefinitionByIdAsync(
        GetScheduledJobDefinitionByIdRequest request,
        CancellationToken ct)
        => GetAsync<GetScheduledJobDefinitionByIdRequest, GetScheduledJobDefinitionByIdResponse>(request, ct);

    public Task<OperationResult<GetNextScheduledJobRunResponse>> GetNextScheduledJobRunAsync(
        GetNextScheduledJobRunRequest request,
        CancellationToken ct)
        => GetAsync<GetNextScheduledJobRunRequest, GetNextScheduledJobRunResponse>(request, ct);

    public Task<OperationResult<CreateScheduledJobDefinitionResponse>> CreateScheduledJobDefinitionAsync(
        CreateScheduledJobDefinitionRequest request,
        CancellationToken ct)
        => PostAsync<CreateScheduledJobDefinitionRequest, CreateScheduledJobDefinitionResponse>(request, ct);

    public Task<OperationResult<UpdateScheduledJobDefinitionResponse>> UpdateScheduledJobDefinitionAsync(
        UpdateScheduledJobDefinitionRequest request,
        CancellationToken ct)
        => PutAsync<UpdateScheduledJobDefinitionRequest, UpdateScheduledJobDefinitionResponse>(request, ct);
}