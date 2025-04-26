using Api.Business.Entities;
using Api.Business.Features.ScheduledJobs;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ScheduledJobDefinitionRepository : IScheduledJobDefinitionRepository
{
    private readonly AppDbContext _context;

    public ScheduledJobDefinitionRepository(AppDbContext context) => _context = context;

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
        def.ScheduledJobDefinitionId = entity.Id;
        // copy the generated Id back if you need it: def.Id = entity.Id;
    }

    public async Task<bool> UpdateAsync(ScheduledJobDefinitionDomainModel def)
    {
        var entity = await _context.ScheduledJobDefinitions
            .Include(d => d.JobOccurrences)
            .ThenInclude(o => o.JobReminders)
            .FirstOrDefaultAsync(d => d.Id == def.ScheduledJobDefinitionId);

        if (entity == null) return false;

        _context.ScheduledJobDefinitions.Update(def.ToEntity());
        return true;
    }
}

// Extension methods to keep mapping code tidy:
internal static class JobDefinitionMappings
{
    public static ScheduledJobDefinitionDomainModel ToDomain(this ScheduledJobDefinition e)
        => new()
        {
            ScheduledJobDefinitionId = e.Id,
            CustomerId = e.CustomerId,
            AnchorDate = e.AnchorDate,
            Title = e.Title,
            Description = e.Description,
            Pattern = new RecurrencePattern(e.Pattern.Frequency, e.Pattern.Interval, e.Pattern.WeekDays),
            JobOccurrences = e.JobOccurrences
                .Select(o => new JobOccurrenceDomainModel
                {
                    Id = o.Id,
                    ScheduledJobDefinitionId = o.ScheduledJobDefinitionId,
                    OccurrenceDate = o.OccurrenceDate,
                    JobReminders = o.JobReminders
                        .Select(r => new JobReminderDomainModel
                        {
                            ReminderDateTime = r.ReminderDateTime,
                            JobOccurrenceId = o.Id,
                            ScheduledJobDefinitionId = o.ScheduledJobDefinitionId,
                            Id = r.Id,
                            IsSent = r.IsSent,
                            SentDate = r.SentDate,
                            Message = r.Message
                        }).ToList()
                })
                .ToList()
        };

    public static ScheduledJobDefinition ToEntity(this ScheduledJobDefinitionDomainModel d)
    {
        var e = new ScheduledJobDefinition
        {
            Id = d.ScheduledJobDefinitionId,
            Title = d.Title,
            Description = d.Description,
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
                    Id = o.Id,
                    ScheduledJobDefinitionId = d.ScheduledJobDefinitionId,
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