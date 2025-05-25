using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Services;
using Api.Infrastructure.Data;
using NCrontab;
using Server.Contracts.Cron;

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
        var nowUtc = DateTime.UtcNow;
        var allDefs = await _repo.ListAllAsync();

        foreach (var def in allDefs)
        {
            // 1) Grab the user’s chosen anchor (or last‐saved anchor)
            var anchorDateTime = def.AnchorDate;

            // 2) Build the CronSchedule once
            var cronSchedule = new CronSchedule(
                CrontabSchedule.Parse(def.CronExpression)
            );

            // 3) Step the anchor forward until the *next* occurrence is in the future
            var nextOccurrence = _calculator.GetNextOccurrence(cronSchedule, anchorDateTime);

            // If that “next” is still <= now, we’ve missed one (or more).
            // Loop until nextUtc > nowUtc.
            while (nextOccurrence < nowUtc)
            {
                // advance the anchor
                anchorDateTime = nextOccurrence;

                // compute from that new anchor
                nextOccurrence = _calculator.GetNextOccurrence(cronSchedule, anchorDateTime);
            }
            
            // todo add check to confirm that we're not starting after once cycle after the current cycle

            // 4) At this point:
            //     anchorUtc = the *last* occurrence ≤ now
            //     nextUtc = the *next* occurrence > now
            //
            //    Persist the moved‐forward anchor:
            def.AnchorDate = anchorDateTime;
            await _repo.UpdateAsync(def);

            // 5) And fire off your new occurrence (nextUtc) if you want to record it now:
            //    (Or you can choose to let callers pull nextUtc themselves.)
            var occ = new JobOccurrenceDomainModel
            {
                OccurrenceDate = nextOccurrence
            };
            def.JobOccurrences.Add(occ);

            await _repo.UpdateAsync(def);
        }

        await _uow.SaveChangesAsync();
    }
}