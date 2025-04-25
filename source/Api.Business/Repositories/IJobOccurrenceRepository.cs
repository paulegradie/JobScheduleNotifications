using Api.Business.Entities;
using Api.ValueTypes;

namespace Api.Business.Repositories;

public interface IJobOccurrenceRepository
{
    /// <summary>
    /// Returns all occurrences for a given job definition.
    /// </summary>
    Task<IEnumerable<JobOccurrenceDomainModel>> ListByDefinitionAsync(
        ScheduledJobDefinitionId jobDefinitionId);

    /// <summary>
    /// Retrieves a single occurrence.
    /// </summary>
    Task<JobOccurrenceDomainModel?> GetByIdAsync(JobOccurrenceId occurrenceId);
}