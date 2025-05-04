using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ScheduledJobDefinitionRepository : IScheduledJobDefinitionRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ScheduledJobDefinitionRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ScheduledJobDefinitionDomainModel?> GetAsync(ScheduledJobDefinitionId id)
    {
        var entity = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(p => p.Pattern)
            .Include(d => d.JobOccurrences)
            .ThenInclude(o => o.JobReminders)
            .FirstOrDefaultAsync(d => d.ScheduledJobDefinitionId == id);
        return entity?.ToDomain();
    }

    public async Task<List<ScheduledJobDefinitionDomainModel>> ListByCustomerAsync(CustomerId customerId)
    {
        var list = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(p => p.Pattern)
            .Include(d => d.JobOccurrences)
            .ThenInclude(o => o.JobReminders)
            .Where(d => d.CustomerId == customerId)
            .ToListAsync();
        return list.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<ScheduledJobDefinitionDomainModel>> ListAllAsync()
    {
        var entities = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(x => x.Pattern)
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
        def.ScheduledJobDefinitionId = entity.ScheduledJobDefinitionId;
        def.Pattern.Id = entity.Pattern.RecurrencePatternId;
        // copy the generated Id back if you need it: def.Id = entity.Id;
    }

    public async Task UpdateAsync(ScheduledJobDefinitionDomainModel def)
    {
        var entity = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(x => x.Pattern)
            .Include(d => d.JobOccurrences)
            .ThenInclude(o => o.JobReminders)
            .Include(scheduledJobDefinition => scheduledJobDefinition.Pattern)
            .FirstOrDefaultAsync(d => d.ScheduledJobDefinitionId == def.ScheduledJobDefinitionId);

        if (entity == null)
            return;

        entity.Title = def.Title;
        entity.Description = def.Description;
        entity.AnchorDate = def.AnchorDate;
        entity.Pattern.Frequency = def.Pattern.Frequency;
        entity.Pattern.Interval = def.Pattern.Interval;
        entity.Pattern.WeekDays = def.Pattern.WeekDays;
        entity.Pattern.DayOfMonth = def.Pattern.DayOfMonth;

        _context.ScheduledJobDefinitions.Update(entity);

        await _context.SaveChangesAsync();
        def.ScheduledJobDefinitionId = entity.ScheduledJobDefinitionId;
        def.CustomerId = entity.CustomerId;
        def.Pattern.Id = entity.Pattern.RecurrencePatternId;
    }
}

// Extension methods to keep mapping code tidy:
internal static class JobDefinitionMappings
{
    public static ScheduledJobDefinitionDomainModel ToDomain(this ScheduledJobDefinition e)
        => new()
        {
            ScheduledJobDefinitionId = e.ScheduledJobDefinitionId,
            CustomerId = e.CustomerId,
            AnchorDate = e.AnchorDate,
            Title = e.Title,
            Description = e.Description,
            Pattern = new RecurrencePatternDomainModel(e.Pattern.Frequency, e.Pattern.Interval, e.Pattern.WeekDays)
            {
                Id = e.Pattern.RecurrencePatternId,
                CronExpression = e.Pattern.CronExpression,
                
            },
            JobOccurrences = e.JobOccurrences
                .Select(o => new JobOccurrenceDomainModel
                {
                    Id = o.JobOccurrenceId,
                    ScheduledJobDefinitionId = o.ScheduledJobDefinitionId,
                    OccurrenceDate = o.OccurrenceDate,
                    JobReminders = o.JobReminders
                        .Select(r => new JobReminderDomainModel
                        {
                            ReminderDateTime = r.ReminderDateTime,
                            JobOccurrenceId = o.JobOccurrenceId,
                            ScheduledJobDefinitionId = o.ScheduledJobDefinitionId,
                            Id = r.JobReminderId,
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
            ScheduledJobDefinitionId = d.ScheduledJobDefinitionId,
            Title = d.Title,
            Description = d.Description,
            CustomerId = d.CustomerId,
            AnchorDate = d.AnchorDate,
            Pattern = new RecurrencePattern
            {
                // RecurrencePatternId = d.Pattern.Id,
                Frequency = d.Pattern.Frequency,
                Interval = d.Pattern.Interval,
                WeekDays = d.Pattern.WeekDays,
                CronExpression = d.Pattern.CronExpression,
                DayOfMonth = d.Pattern.DayOfMonth,
                ScheduledJobDefinitionId = d.ScheduledJobDefinitionId
            },
            JobOccurrences = d.JobOccurrences
                .Select(o => new JobOccurrence
                {
                    JobOccurrenceId = o.Id,
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