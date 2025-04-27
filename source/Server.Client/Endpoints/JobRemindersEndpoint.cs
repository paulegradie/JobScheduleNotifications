using Server.Client.Base;
using Server.Contracts.Common;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Reminders;
using Server.Contracts.Endpoints.Reminders.Contracts;

namespace Server.Client.Endpoints;

/// <summary>
/// HTTP-based implementation of <see cref="IJobRemindersEndpoint"/>,
/// using the shared <see cref="EndpointBase"/> logic.
/// </summary>
internal class JobRemindersEndpoint : EndpointBase, IJobRemindersEndpoint
{
    public JobRemindersEndpoint(HttpClient client)
        : base(client)
    {
    }

    public Task<OperationResult<ListJobRemindersResponse>> ListJobRemindersAsync(
        ListJobRemindersRequest request,
        CancellationToken cancellationToken)
        => GetAsync<ListJobRemindersRequest, ListJobRemindersResponse>(
            request,
            cancellationToken);

    public Task<OperationResult<GetJobReminderByIdResponse>> GetJobReminderByIdAsync(
        GetJobReminderByIdRequest request,
        CancellationToken cancellationToken)
        => GetAsync<GetJobReminderByIdRequest, GetJobReminderByIdResponse>(
            request,
            cancellationToken);

    public Task<OperationResult<CreateJobReminderResponse>> CreateJobReminderAsync(
        CreateJobReminderRequest request,
        CancellationToken cancellationToken)
        => PostAsync<CreateJobReminderRequest, CreateJobReminderResponse>(
            request,
            cancellationToken);

    public Task<OperationResult<UpdateJobReminderResponse>> UpdateJobReminderAsync(
        UpdateJobReminderRequest request,
        CancellationToken cancellationToken)
        => PutAsync<UpdateJobReminderRequest, UpdateJobReminderResponse>(
            request,
            cancellationToken);

    public async Task<OperationResult> DeleteJobReminderAsync(
        DeleteJobReminderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await DeleteAsync<DeleteJobReminderRequest, Unit>(
            request,
            cancellationToken);

        return result.IsSuccess
            ? OperationResult.Success(result.StatusCode)
            : OperationResult.Failure(result.ErrorMessage ?? string.Empty, result.StatusCode);
    }

    public Task<OperationResult<AcknowledgeJobReminderResponse>> AcknowledgeJobReminderAsync(
        AcknowledgeJobReminderRequest request,
        CancellationToken cancellationToken)
        => PutAsync<AcknowledgeJobReminderRequest, AcknowledgeJobReminderResponse>(
            request,
            cancellationToken);
}