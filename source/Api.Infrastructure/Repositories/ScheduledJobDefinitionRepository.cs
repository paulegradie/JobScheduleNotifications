using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ScheduledJobDefinitionRepository : IScheduledJobDefinitionRepository
{
    private readonly AppDbContext _context;

    public ScheduledJobDefinitionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduledJobDefinitionDomainModel?> GetAsync(ScheduledJobDefinitionId id)
    {
        var entity = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(d => d.JobOccurrences)
            .ThenInclude(p => p.JobCompletedPhotos)
            .Include(d => d.JobReminders)
            .FirstOrDefaultAsync(d => d.ScheduledJobDefinitionId == id);
        return entity?.ToDomain();
    }

    public async Task<List<ScheduledJobDefinitionDomainModel>> ListByCustomerAsync(CustomerId customerId)
    {
        var list = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(d => d.JobOccurrences)
            .ThenInclude(x => x.JobCompletedPhotos)
            .Include(o => o.JobReminders)
            .Where(d => d.CustomerId == customerId)
            .ToListAsync();
        return list.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<ScheduledJobDefinitionDomainModel>> ListAllAsync()
    {
        var entities = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(d => d.JobOccurrences)
            .ThenInclude(x => x.JobCompletedPhotos)
            .Include(o => o.JobReminders)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task AddAsync(ScheduledJobDefinitionDomainModel def)
    {
        // You’ll need a reverse-map: domain → EF entity
        var entity = def.ToEntity();
        await _context.ScheduledJobDefinitions.AddAsync(entity);
        def.ScheduledJobDefinitionId = entity.ScheduledJobDefinitionId;
        def.CronExpression = entity.CronExpression;
    }

    public async Task UpdateAsync(ScheduledJobDefinitionDomainModel def)
    {
        var entity = await _context.ScheduledJobDefinitions
            .Include(x => x.Customer)
            .Include(d => d.JobOccurrences)
            .ThenInclude(x => x.JobCompletedPhotos)
            .Include(o => o.JobReminders)
            .FirstOrDefaultAsync(d => d.ScheduledJobDefinitionId == def.ScheduledJobDefinitionId);

        if (entity == null)
            return;

        entity.Title = def.Title;
        entity.Description = def.Description;
        entity.AnchorDate = def.AnchorDate;

        _context.ScheduledJobDefinitions.Update(entity);

        await _context.SaveChangesAsync();
        def.ScheduledJobDefinitionId = entity.ScheduledJobDefinitionId;
        def.CustomerId = entity.CustomerId;
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
            CronExpression = e.CronExpression,
            JobOccurrences = e.JobOccurrences
                .Select(o => new JobOccurrenceDomainModel
                {
                    Id = o.JobOccurrenceId,
                    ScheduledJobDefinitionId = o.ScheduledJobDefinitionId,
                    OccurrenceDate = o.OccurrenceDate,
                    CustomerId = e.CustomerId,
                    JobDescription = e.Description,
                    JobTitle = e.Title,
                    CompletedDate = o.CompletedDate,
                    MarkedAsComplete = o.MarkedAsCompleted,
                    JobCompletedPhotoDomainModel = o.JobCompletedPhotos.Select(p => new JobCompletedPhotoDomainModel
                    {
                        CustomerId = p.CustomerId,
                        JobCompletedPhotoId = p.JobCompletedPhotoId,
                        JobOccurrenceId = p.JobOccurrenceId,
                        PhotoUri = p.LocalFilePath
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
            CronExpression = d.CronExpression,
            JobOccurrences = d.JobOccurrences
                .Select(o => new JobOccurrence
                {
                    JobOccurrenceId = o.Id,
                    ScheduledJobDefinitionId = d.ScheduledJobDefinitionId,
                    OccurrenceDate = o.OccurrenceDate
                })
                .ToList()
        };
        return e;
    }
}