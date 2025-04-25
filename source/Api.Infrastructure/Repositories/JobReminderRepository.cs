using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    /// <summary>
    /// EF-backed implementation of <see cref="IJobReminderRepository"/>,
    /// correctly traversing Occurrence → Definition → Customer.
    /// </summary>
    public class JobReminderRepository : IJobReminderRepository
    {
        private readonly AppDbContext _context;

        private readonly ICrudRepository<JobReminder, JobReminderId> _reminders;
        private readonly ICrudRepository<JobOccurrence, JobOccurrenceId> _occurrences;

        public JobReminderRepository(
            ICrudRepository<JobReminder, JobReminderId> reminders,
            ICrudRepository<JobOccurrence, JobOccurrenceId> occurrences)
        {
            _reminders   = reminders;
            _occurrences = occurrences;
        }
        
        public JobReminderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobReminderDomainModel>> ListByOccurrenceAsync(
            JobOccurrenceId occurrenceId,
            bool? isSent = null)
        {
            var q = _context.Set<JobReminder>()
                .Include(r => r.JobOccurrence)
                .Where(r => r.JobOccurrenceId == occurrenceId);

            if (isSent.HasValue)
                q = q.Where(r => r.IsSent == isSent.Value);

            var list = await q.ToListAsync();
            return list.Select(ToDomain);
        }

        public async Task<IEnumerable<JobReminderDomainModel>> ListByJobDefinitionAsync(
            ScheduledJobDefinitionId jobDefinitionId, bool? isSent = null)
        {
            // load all occurrences for that definition
            var occIds = await _occurrences.Query()
                .Where(o => o.ScheduledJobDefinitionId == jobDefinitionId)
                .Select(o => o.Id)
                .ToArrayAsync();

            var q = _reminders.Query()
                .Where(r => occIds.Contains(r.JobOccurrenceId));

            if (isSent.HasValue)
                q = q.Where(r => r.IsSent == isSent.Value);

            var list = await q.ToListAsync();
            return list.Select(e => e.ToDomain());
        }

        public async Task<IEnumerable<JobReminderDomainModel>> ListByCustomerAsync(
            CustomerId customerId,
            bool? isSent = null)
        {
            var q = _context.Set<JobReminder>()
                .Include(r => r.JobOccurrence)
                    .ThenInclude(o => o.ScheduledJobDefinition)
                .Where(r => r.JobOccurrence.ScheduledJobDefinition.CustomerId == customerId);

            if (isSent.HasValue)
                q = q.Where(r => r.IsSent == isSent.Value);

            var list = await q.ToListAsync();
            return list.Select(ToDomain);
        }

        public async Task<IEnumerable<JobReminderDomainModel>> ListUnsentAsync()
        {
            var list = await _context.Set<JobReminder>()
                .Include(r => r.JobOccurrence)
                .Where(r => !r.IsSent)
                .ToListAsync();

            return list.Select(ToDomain);
        }

        public async Task<IEnumerable<JobReminderDomainModel>> ListDueAsync(DateTime upTo)
        {
            var list = await _context.Set<JobReminder>()
                .Include(r => r.JobOccurrence)
                .Where(r => r.ReminderDateTime <= upTo)
                .ToListAsync();

            return list.Select(ToDomain);
        }

        public async Task<JobReminderDomainModel?> GetByIdAsync(JobReminderId id)
        {
            var e = await _context.Set<JobReminder>()
                .Include(r => r.JobOccurrence)
                .FirstOrDefaultAsync(r => r.Id == id);

            return e == null ? null : ToDomain(e);
        }

        public async Task AddAsync(JobReminderDomainModel reminder)
        {
            var e = new JobReminder
            {
                Id               = reminder.Id,
                JobOccurrenceId  = reminder.JobOccurrenceId,
                ReminderDateTime = reminder.ReminderDateTime,
                Message          = reminder.Message,
                IsSent           = reminder.IsSent,
                SentDate         = reminder.SentDate
            };

            _context.Add(e);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobReminderDomainModel reminder)
        {
            var e = await _context.Set<JobReminder>()
                .FirstOrDefaultAsync(r => r.Id == reminder.Id)
                ?? throw new InvalidOperationException($"Reminder {reminder.Id} not found");

            e.ReminderDateTime = reminder.ReminderDateTime;
            e.Message          = reminder.Message;
            e.IsSent           = reminder.IsSent;
            e.SentDate         = reminder.SentDate;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(JobReminderDomainModel reminder)
        {
            var e = await _context.Set<JobReminder>()
                .FirstOrDefaultAsync(r => r.Id == reminder.Id)
                ?? throw new InvalidOperationException($"Reminder {reminder.Id} not found");

            _context.Remove(e);
            await _context.SaveChangesAsync();
        }

        private static JobReminderDomainModel ToDomain(JobReminder e)
            => new JobReminderDomainModel
            {
                Id                       = e.Id,
                JobOccurrenceId          = e.JobOccurrenceId,
                ScheduledJobDefinitionId = e.JobOccurrence.ScheduledJobDefinitionId,
                ReminderDateTime         = e.ReminderDateTime,
                Message                  = e.Message,
                IsSent                   = e.IsSent,
                SentDate                 = e.SentDate
            };
    }
}
