using Api.Business.Entities;
using Api.Business.Features.ScheduledJobs;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class JobDefinitionRepository : IJobDefinitionRepository
{
    private readonly AppDbContext _context;

    public JobDefinitionRepository(AppDbContext context) => _context = context;

    public async Task<ScheduledJobDefinitionDomainModel?> GetAsync(ScheduledJobDefinitionId id)
    {
        var entity = await _context.ScheduledJobDefinitions
            .Include(d => d.JobOccurrences)
            .ThenInclude(o => o.JobReminders)
            .FirstOrDefaultAsync(d => d.Id == id);
        return entity?.ToDomain();
    }

    public async Task<List<ScheduledJobDefinitionDomainModel>> ListByCustomerAsync(CustomerId customerId)
    {
        var list = await _context.ScheduledJobDefinitions
            .Include(d => d.JobOccurrences)
            .ThenInclude(o => o.JobReminders)
            .Where(d => d.CustomerId == customerId)
            .ToListAsync();
        return list.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<ScheduledJobDefinitionDomainModel>> ListAllAsync()
    {
        var entities = await _context.ScheduledJobDefinitions
            .Include(d => d.JobOccurrences)
            .ThenInclude(o => o.JobReminders)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task AddAsync(ScheduledJobDefinitionDomainModel def)
    {
        // You’ll need a reverse-map: domain → EF entity
        var entity = def.ToEntity();
        await _context.ScheduledJobDefinitions.AddAsync(entity);
        // copy the generated Id back if you need it: def.Id = entity.Id;
    }

    public Task UpdateAsync(ScheduledJobDefinitionDomainModel def)
    {
        var entity = def.ToEntity();
        _context.ScheduledJobDefinitions.Update(entity);
        return Task.CompletedTask;
    }
}

// Extension methods to keep mapping code tidy:
internal static class JobDefinitionMappings
{
    public static ScheduledJobDefinitionDomainModel ToDomain(this ScheduledJobDefinition e)
        => new()
        {
            Id = e.Id,
            CustomerId = e.CustomerId,
            AnchorDate = e.AnchorDate,
            Pattern = new RecurrencePattern(e.Pattern.Frequency, e.Pattern.Interval, e.Pattern.WeekDays),
            JobOccurrences = e.JobOccurrences
                .Select(o => new JobOccurrenceDomainModel
                {
                    OccurrenceDate = o.OccurrenceDate,
                    JobReminders = o.JobReminders
                        .Select(r => new JobReminderDomainModel
                        {
                            ReminderDateTime = r.ReminderDateTime
                            // …etc
                        }).ToList()
                })
                .ToList()
        };

    public static ScheduledJobDefinition ToEntity(this ScheduledJobDefinitionDomainModel d)
    {
        var e = new ScheduledJobDefinition
        {
            Id = d.Id,
            CustomerId = d.CustomerId,
            AnchorDate = d.AnchorDate,
            Pattern = new RecurrencePattern
            {
                Frequency = d.Pattern.Frequency,
                Interval = d.Pattern.Interval,
                WeekDays = d.Pattern.WeekDays
            },
            JobOccurrences = d.JobOccurrences
                .Select(o => new JobOccurrence
                {
                    OccurrenceDate = o.OccurrenceDate,
                    JobReminders = o.JobReminders
                        .Select(r => new JobReminder
                        {
                            ReminderDateTime = r.ReminderDateTime
                        }).ToList()
                }).ToList()
        };
        return e;
    }
}