using Server.Contracts.Client.Endpoints.ScheduledJobs.Contracts;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

/// <summary>
/// Client-side interface for interacting with the Scheduled Jobs API.
/// </summary>
public interface IScheduledJobsEndpoint
{
    /// <summary>
    /// List all job definitions for a given customer.
    /// </summary>
    Task<OperationResult<ListJobDefinitionsByCustomerIdResponse>> ListJobDefinitionsAsync(
        ListJobDefinitionsByCustomerIdRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get a specific job definition by its ID.
    /// </summary>
    Task<OperationResult<GetScheduledJobDefinitionByIdResponse>> GetScheduledJobDefinitionByIdAsync(
        GetScheduledJobDefinitionByIdRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get the next run date/time for a job definition.
    /// </summary>
    Task<OperationResult<GetNextScheduledJobRunResponse>> GetNextScheduledJobRunAsync(
        GetNextScheduledJobRunRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Create a new scheduled job definition.
    /// </summary>
    Task<OperationResult<CreateScheduledJobDefinitionResponse>> CreateScheduledJobDefinitionAsync(
        CreateScheduledJobDefinitionRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Update an existing scheduled job definition.
    /// </summary>
    Task<OperationResult<UpdateScheduledJobDefinitionResponse>> UpdateScheduledJobDefinitionAsync(
        UpdateScheduledJobDefinitionRequest request,
        CancellationToken cancellationToken);
}