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
            .Include(o => o.JobReminders)
            .ToListAsync();

        return entities.Select(ToDomain);
    }

    public async Task<JobOccurrenceDomainModel?> GetByIdAsync(JobOccurrenceId occurrenceId)
    {
        var e = await _context.JobOccurrences
            .Include(o => o.JobReminders)
            .FirstOrDefaultAsync(o => o.Id == occurrenceId);

        return e is null ? null : ToDomain(e);
    }

    private static JobOccurrenceDomainModel ToDomain(JobOccurrence e)
        => new JobOccurrenceDomainModel
        {
            Id = e.Id,
            ScheduledJobDefinitionId = e.ScheduledJobDefinitionId,
            OccurrenceDate = e.OccurrenceDate,
            JobReminders = e.JobReminders
                .Select(r => new JobReminderDomainModel
                {
                    Id = r.Id,
                    JobOccurrenceId = r.JobOccurrenceId,
                    ScheduledJobDefinitionId = e.ScheduledJobDefinitionId,
                    ReminderDateTime = r.ReminderDateTime,
                    Message = r.Message,
                    IsSent = r.IsSent,
                    SentDate = r.SentDate
                })
                .ToList()
        };
}