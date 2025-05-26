using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobOccurence.Contracts;

namespace Mobile.Infrastructure.Repositories;

public class JobOccurrenceRepository : IJobOccurrenceRepository
{
    private readonly IServerClient _serverClient;

    public JobOccurrenceRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<OperationResult<GetJobOccurrenceByIdResponse>> GetOccurrenceByIdAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        JobOccurrenceId id,
        CancellationToken ct = default)
    {
        var result = await _serverClient.JobOccurrences.GetAsync(new GetJobOccurrenceByIdRequest(customerId, scheduledJobDefinitionId, id), ct);
        return result;
    }

    public async Task<OperationResult<UpdateJobOccurrenceResponse>> MarkOccurrenceAsCompletedAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        JobOccurrenceId jobOccurrenceId,
        CancellationToken ct = default)
    {
        var result = await GetOccurrenceByIdAsync(customerId, scheduledJobDefinitionId, jobOccurrenceId, ct);
        if (!result.IsSuccess) throw new Exception("Failed to get occurrence");
        var occurrenceDto = result.Value.JobOccurrence;

        var newDto = occurrenceDto with { JobDescription = occurrenceDto.JobDescription, JobOccurrenceId = jobOccurrenceId, CompletedDate = DateTime.UtcNow, MarkedAsCompleted = true };

        var request = new UpdateJobOccurrenceRequest(
            CustomerId: occurrenceDto.CustomerId,
            ScheduledJobDefinitionId: occurrenceDto.ScheduledJobDefinitionId,
            JobOccurrenceId: jobOccurrenceId,
            OccurrenceDate: newDto.OccurrenceDate,
            MarkAsCompleted: newDto.MarkedAsCompleted,
            JobTitle: newDto.JobTitle,
            JobDescription: newDto.JobDescription
        );

        var response = await _serverClient.JobOccurrences.UpdateAsync(request, ct);
        return response;
    }


    public async Task<OperationResult<CreateJobOccurrenceResponse>> CreateNewOccurrence(
        CustomerId customerId,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        CancellationToken ct = default)
    {
        var occurrenceDate = DateTime.UtcNow;
        var result = await _serverClient.JobOccurrences.CreateAsync(new CreateJobOccurrenceRequest(customerId, scheduledJobDefinitionId, occurrenceDate), ct);
        return result;
    }
}