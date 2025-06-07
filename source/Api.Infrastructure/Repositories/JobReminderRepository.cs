using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Repositories.Internal;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Services;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly UserManager<ApplicationUserRecord> _userManager;
    private readonly AppDbContext _dbContext;

    public UserSettingsRepository(ICurrentUserContext currentUserContext, UserManager<ApplicationUserRecord> userManager, AppDbContext dbContext)
    {
        _currentUserContext = currentUserContext;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<CurrentUserSettings> GetCurrentUserSettings()
    {
        var user = await _dbContext
            .ApplicationUsers
            .Include(applicationUserRecord => applicationUserRecord.OrganizationUsers)
            .ThenInclude(organizationUser => organizationUser.Organization)
            .FirstOrDefaultAsync(x => x.Id == _currentUserContext.UserId);
        if (user is null) throw new Exception("No user found");

        var organization = user.OrganizationUsers.Single();
        var businessName = organization.Organization.Name;

        return new CurrentUserSettings
        {
            BusinessName = businessName,
            UserName = user.UserName ?? user.Email ?? "Could not find user name or email"
        };
    }
}

public class JobReminderRepository : IJobReminderRepository
{
    private readonly AppDbContext _context;

    private readonly ICrudRepository<JobReminder, JobReminderId> _reminders;
    private readonly ICrudRepository<JobOccurrence, JobOccurrenceId> _occurrences;

    public JobReminderRepository(
        ICrudRepository<JobReminder, JobReminderId> reminders,
        ICrudRepository<JobOccurrence, JobOccurrenceId> occurrences,
        AppDbContext context)
    {
        _reminders = reminders;
        _occurrences = occurrences;
        _context = context;
    }

    public async Task<IEnumerable<JobReminderDomainModel>> ListByOccurrenceAsync(
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        bool? isSent = null)
    {
        var q = _context.Set<JobReminder>()
            .Include(r => r.ScheduledJobDefinition)
            .Where(r => r.ScheduledJobDefinitionId == scheduledJobDefinitionId);

        if (isSent.HasValue)
            q = q.Where(r => r.IsSent == isSent.Value);

        var list = await q.ToListAsync();
        return list.Select(ToDomain);
    }

    public async Task<IEnumerable<JobReminderDomainModel>> ListByJobDefinitionAsync(
        ScheduledJobDefinitionId scheduledJobDefinitionId, bool? isSent = null)
    {
        var q = _reminders.Query()
            .Where(r => r.ScheduledJobDefinitionId == scheduledJobDefinitionId);

        if (isSent.HasValue)
            q = q.Where(r => r.IsSent == isSent.Value);

        var list = await q.ToListAsync();
        return list.Select(ToDomain);
    }

    public async Task<IEnumerable<JobReminderDomainModel>> ListByCustomerAsync(
        CustomerId customerId,
        bool? isSent = null)
    {
        var q = _context.Set<JobReminder>()
            .Include(r => r.ScheduledJobDefinition)
            .ThenInclude(o => o.Customer)
            .Where(r => r.ScheduledJobDefinition.CustomerId == customerId);

        if (isSent.HasValue)
            q = q.Where(r => r.IsSent == isSent.Value);

        var list = await q.ToListAsync();
        return list.Select(ToDomain);
    }

    public async Task<IEnumerable<JobReminderDomainModel>> ListUnsentAsync(ScheduledJobDefinitionId scheduledJobDefinitionId)
    {
        var list = await _context.Set<JobReminder>()
            .Include(r => r.ScheduledJobDefinition)
            .ThenInclude(o => o.Customer)
            .Where(r => !r.IsSent)
            .ToListAsync();

        return list.Select(ToDomain);
    }

    public async Task<IEnumerable<JobReminderDomainModel>> ListDueAsync(DateTime upTo)
    {
        var list = await _context.Set<JobReminder>()
            .Include(r => r.ScheduledJobDefinition)
            .Where(r => r.ReminderDateTime <= upTo)
            .ToListAsync();

        return list.Select(ToDomain);
    }

    public async Task<JobReminderDomainModel?> GetByIdAsync(JobReminderId id)
    {
        var e = await _context.Set<JobReminder>()
            .Include(r => r.ScheduledJobDefinition)
            .FirstOrDefaultAsync(r => r.JobReminderId == id);

        return e == null ? null : ToDomain(e);
    }

    public async Task AddAsync(JobReminderDomainModel reminder)
    {
        var e = new JobReminder
        {
            ReminderDateTime = reminder.ReminderDateTime,
            Message = reminder.Message,
            IsSent = reminder.IsSent,
            SentDate = reminder.SentDate,
            ScheduledJobDefinitionId = reminder.ScheduledJobDefinitionId,
        };

        _context.Add(e);
        await _context.SaveChangesAsync();
        reminder.SetJobReminderId(e.JobReminderId);
        reminder.SetScheduledJobDefinitionId(e.ScheduledJobDefinitionId);
    }

    public async Task UpdateAsync(JobReminderDomainModel reminder)
    {
        var e = await _context.Set<JobReminder>()
                    .FirstOrDefaultAsync(r => r.JobReminderId == reminder.JobReminderId)
                ?? throw new InvalidOperationException($"Reminder {reminder.JobReminderId} not found");

        e.ReminderDateTime = reminder.ReminderDateTime;
        e.Message = reminder.Message;
        e.IsSent = reminder.IsSent;
        e.SentDate = reminder.SentDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(JobReminderDomainModel reminder)
    {
        var e = await _context.Set<JobReminder>()
                    .FirstOrDefaultAsync(r => r.JobReminderId == reminder.JobReminderId)
                ?? throw new InvalidOperationException($"Reminder {reminder.JobReminderId} not found");

        _context.Remove(e);
        await _context.SaveChangesAsync();
    }

    private static JobReminderDomainModel ToDomain(JobReminder e)
    {
        var reminder = new JobReminderDomainModel(
            scheduledJobDefinitionId: e.ScheduledJobDefinitionId,
            reminderDateTime: e.ReminderDateTime,
            message: e.Message,
            isSent: e.IsSent,
            sentDate: e.SentDate);

        reminder.SetJobReminderId(e.JobReminderId);
        return reminder;
    }
}