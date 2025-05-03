using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Services;
using Api.Infrastructure.Data;
using Api.ValueTypes.Enums;

namespace Api.Infrastructure.Services;

public class JobSchedulingService : IJobSchedulingService
{
    private readonly IScheduledJobDefinitionRepository _repo;
    private readonly IRecurrenceCalculator _calculator;
    private readonly AppDbContext _uow; // or your DbContext

    public JobSchedulingService(
        IScheduledJobDefinitionRepository repo,
        IRecurrenceCalculator calc,
        AppDbContext uow)
    {
        _repo = repo;
        _calculator = calc; 
        _uow = uow;
    }

    public async Task ProcessDueJobsAsync()
    {
        var allDefs = await _repo.ListAllAsync(); // ← now exists

        foreach (var def in allDefs)
        {
            var lastOccurrenceUtc = def.JobOccurrences
                                        .OrderByDescending(o => o.OccurrenceDate)
                                        .FirstOrDefault()?.OccurrenceDate
                                    ?? def.AnchorDate;

            var nextUtc = _calculator.GetNextOccurrence(
                def.Pattern,
                def.AnchorDate,
                lastOccurrenceUtc);

            if (nextUtc <= DateTime.UtcNow)
            {
                // 1) create the new occurrence
                var occ = new JobOccurrenceDomainModel
                {
                    OccurrenceDate = nextUtc,
                    JobReminders = new List<JobReminderDomainModel>()
                };

                // 2) wire up reminders (example: 1‐day before)
                var reminderDate = nextUtc.AddDays(-1);
                occ.JobReminders.Add(new JobReminderDomainModel
                {
                    ReminderDateTime = reminderDate
                });

                // 3) attach into the aggregate
                def.JobOccurrences.Add(occ);

                // 4) tell the repo you’ve changed it
                await _repo.UpdateAsync(def);
            }
        }

        // finally, persist all of your adds/updates in one shot
        await _uow.SaveChangesAsync();
    }

    private DateTime GetNextWeekly(
        DateTime anchor,
        WeekDay dayOfWeek,
        int intervalWeeks,
        DateTime afterUtc)
    {
        // start counting from the anchor’s week
        var weeksSinceAnchor = (int)((afterUtc.Date - anchor.Date).TotalDays / 7);

        // find the next *candidate* date, scanning one day at a time
        var candidate = afterUtc.Date.AddDays(1);
        while (true)
        {
            var weeksFromAnchor = (int)((candidate - anchor.Date).TotalDays / 7);
            var inCorrectInterval = (weeksFromAnchor % intervalWeeks) == 0;
            var isCorrectDay = dayOfWeek.HasFlag((WeekDay)(1 << (int)candidate.DayOfWeek));

            if (inCorrectInterval && isCorrectDay)
                return candidate.Add(afterUtc.TimeOfDay);

            candidate = candidate.AddDays(1);
        }
    }
}