using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.ScheduledJobs.Contracts;
using Server.Contracts.Dtos;

namespace Mobile.Core.Domain.Repositories;

public class JobRepository : IJobRepository
{
    private readonly IServerClient _serverClient;

    public JobRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<OperationResult<IEnumerable<ScheduledJobDefinitionDto>>> GetJobsByCustomerAsync(
        CustomerId customerId,
        CancellationToken ct = default)
    {
        var request = new ListJobDefinitionsByCustomerIdRequest(customerId);
        var result = await _serverClient.ScheduledJobs.ListJobDefinitionsAsync(request, ct);

        if (result.IsSuccess)
        {
            return OperationResult<IEnumerable<ScheduledJobDefinitionDto>>.Success(
                result.Value.ScheduledJobDefinitions,
                result.StatusCode);
        }

        return OperationResult<IEnumerable<ScheduledJobDefinitionDto>>.Failure(
            result.ErrorMessage,
            result.StatusCode);
    }

    public async Task<OperationResult<ScheduledJobDefinitionDto>> GetJobByIdAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId id,
        CancellationToken ct = default)
    {
        var request = new GetScheduledJobDefinitionByIdRequest(customerId, id);
        var result = await _serverClient.ScheduledJobs.GetScheduledJobDefinitionByIdAsync(request, ct);

        if (result.IsSuccess)
        {
            return OperationResult<ScheduledJobDefinitionDto>.Success(
                result.Value.ScheduledJobDefinitionDto,
                result.StatusCode);
        }

        return OperationResult<ScheduledJobDefinitionDto>.Failure(
            result.ErrorMessage,
            result.StatusCode);
    }

    public async Task<OperationResult<ScheduledJobDefinitionDto>> CreateJobAsync(
        CreateScheduledJobDefinitionDto job,
        CancellationToken ct = default)
    {
        var request = new CreateScheduledJobDefinitionRequest(
            job.CustomerId,
            job.Title,
            job.Description,
            job.Frequency,
            job.Interval,
            job.WeekDays,
            job.DayOfMonth,
            job.CronExpression,
            job.AnchorDate);

        var result = await _serverClient.ScheduledJobs.CreateScheduledJobDefinitionAsync(request, ct);

        if (result.IsSuccess)
        {
            return OperationResult<ScheduledJobDefinitionDto>.Success(
                result.Value.JobDefinition,
                result.StatusCode);
        }

        return OperationResult<ScheduledJobDefinitionDto>.Failure(
            result.ErrorMessage,
            result.StatusCode);
    }

    public async Task<OperationResult<ScheduledJobDefinitionDto>> UpdateJobAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId id,
        UpdateJobDto job,
        CancellationToken ct = default)
    {
        var request = UpdateScheduledJobDefinitionRequest.CreateBuilder(customerId, id)
            .WithDayOfMonth(job.DayOfMonth)
            .WithCronExpression(job.CronExpression)
            .WithDescription(job.Description)
            .WithTitle(job.Title)
            .WithFrequency(job.Frequency)
            .WithInterval(job.Interval)
            .WithWeekDays(job.WeekDays)
            .WithDayOfMonth(job.DayOfMonth)
            .WithCronExpression(job.CronExpression)
            .WithAnchorDate(job.AnchorDate)
            .Build();


        var result = await _serverClient.ScheduledJobs.UpdateScheduledJobDefinitionAsync(request, ct);

        if (result.IsSuccess)
        {
            return OperationResult<ScheduledJobDefinitionDto>.Success(
                result.Value.ScheduledJobDefinitionDto,
                result.StatusCode);
        }

        return OperationResult<ScheduledJobDefinitionDto>.Failure(
            result.ErrorMessage,
            result.StatusCode);
    }

    public async Task<OperationResult> DeleteJobAsync(
        ScheduledJobDefinitionId id,
        CancellationToken ct = default)
    {
        // Assuming delete is not supported by endpoint
        return OperationResult.Failure("Delete not supported", System.Net.HttpStatusCode.BadRequest);
    }

    public async Task<OperationResult<ScheduledJobDefinitionDto>> MarkJobAsCompletedAsync(
        ScheduledJobDefinitionId id,
        CancellationToken ct = default)
    {
        // Assuming mark-completed not supported
        return OperationResult<ScheduledJobDefinitionDto>.Failure(
            "Mark as completed not supported",
            System.Net.HttpStatusCode.BadRequest);
    }
}