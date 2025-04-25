using Server.Client.Base;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.Reminders;
using Server.Contracts.Client.Endpoints.Reminders.Contracts;

namespace Server.Client.Endpoints;

internal class JobRemindersEndpoint : EndpointBase, IJobRemindersEndpoint
{
    public JobRemindersEndpoint(HttpClient client) : base(client)
    {
    }

    public Task<OperationResult<ListJobRemindersResponse>> ListJobRemindersAsync(ListJobRemindersRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<GetJobReminderByIdResponse>> GetJobReminderByIdAsync(GetJobReminderByIdRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<CreateJobReminderResponse>> CreateJobReminderAsync(CreateJobReminderRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UpdateJobReminderResponse>> UpdateJobReminderAsync(UpdateJobReminderRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> DeleteJobReminderAsync(DeleteJobReminderRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<AcknowledgeJobReminderResponse>> AcknowledgeJobReminderAsync(AcknowledgeJobReminderRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}