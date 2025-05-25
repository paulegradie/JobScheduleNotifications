using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

/// <summary>
/// EF-backed implementation of <see cref="IJobOccurrenceRepository"/>.
/// </summary>
public class JobOccurrenceRepository : IJobOccurrenceRepository
{
    private readonly AppDbContext _context;

    public JobOccurrenceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobOccurrenceDomainModel>> ListByDefinitionAsync(
        ScheduledJobDefinitionId jobDefinitionId)
    {
        var entities = await _context.JobOccurrences
            .Where(o => o.ScheduledJobDefinitionId == jobDefinitionId)
            .ToListAsync();

        return entities.Select(ToDomain);
    }

    public async Task<JobOccurrenceDomainModel?> GetByIdAsync(JobOccurrenceId occurrenceId)
    {
        var entity = await _context.JobOccurrences
            .FirstOrDefaultAsync(o => o.JobOccurrenceId == occurrenceId);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(JobOccurrenceDomainModel occurrence)
    {
        var entity = occurrence.ToEntity();
        var def = await _context.ScheduledJobDefinitions.SingleAsync(x => x.ScheduledJobDefinitionId == occurrence.ScheduledJobDefinitionId);
        def.JobOccurrences.Add(entity);
        await _context.SaveChangesAsync();
        occurrence.Id = entity.JobOccurrenceId;
        occurrence.ScheduledJobDefinitionId = def.ScheduledJobDefinitionId;
    }

    public async Task UpdateAsync(JobOccurrenceDomainModel occurrence)
    {
        var entity = await _context.JobOccurrences.FirstOrDefaultAsync(o => o.JobOccurrenceId == occurrence.Id);

        if (entity == null)
            return;

        entity.OccurrenceDate = occurrence.OccurrenceDate;
        entity.ScheduledJobDefinitionId = occurrence.ScheduledJobDefinitionId;
        _context.JobOccurrences.Update(entity);
        await _context.SaveChangesAsync();
        occurrence.Id = entity.JobOccurrenceId;
    }

    public async Task DeleteAsync(JobOccurrenceDomainModel occurrence)
    {
        var entity = await _context.JobOccurrences
            .FirstOrDefaultAsync(o => o.JobOccurrenceId == occurrence.Id);

        if (entity == null)
            return;

        _context.JobOccurrences.Remove(entity);
        await _context.SaveChangesAsync();
    }

    private static JobOccurrenceDomainModel ToDomain(JobOccurrence e)
        => new JobOccurrenceDomainModel
        {
            Id = e.JobOccurrenceId,
            ScheduledJobDefinitionId = e.ScheduledJobDefinitionId,
            OccurrenceDate = e.OccurrenceDate
        };
}

internal static class JobOccurrenceMappings
{
    /// <summary>
    /// Map EF entity to domain model.
    /// </summary>
    public static JobOccurrenceDomainModel ToDomain(this JobOccurrence e)
        => new()
        {
            Id = e.JobOccurrenceId,
            ScheduledJobDefinitionId = e.ScheduledJobDefinitionId,
            OccurrenceDate = e.OccurrenceDate
        };

    /// <summary>
    /// Map domain model back to EF entity, including nested reminders.
    /// </summary>
    public static JobOccurrence ToEntity(this JobOccurrenceDomainModel d)
        => new()
        {
            JobOccurrenceId = d.Id,
            CustomerId = d.CustomerId,
            CompletedDate = d.CompletedDate,
            ScheduledJobDefinitionId = d.ScheduledJobDefinitionId,
            OccurrenceDate = d.OccurrenceDate
        };
}