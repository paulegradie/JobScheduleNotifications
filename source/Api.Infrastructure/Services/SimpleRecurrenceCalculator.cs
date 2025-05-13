using Api.Business.Entities;
using Api.Business.Services;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes.Enums;
using NCrontab;
using Server.Contracts.Cron;

namespace Api.Infrastructure.Services;

public class SimpleRecurrenceCalculator : IRecurrenceCalculator
{
    /// <returns>Next occurrence in UTC.</returns>
    public DateTime GetNextOccurrence(CronSchedule schedule, DateTime anchorDate)
    {
        if (schedule == null)
        {
            throw new ArgumentNullException(nameof(schedule));
        }

        // Build the standard 5-field cron string
        var expression = schedule.ToString();

        // Parse with NCrontab (5-field cron) using local time behavior
        var cron = CrontabSchedule.Parse(expression);

        // NCrontab works in local time; convert 'afterUtc' to local:
        var nextLocal = cron.GetNextOccurrence(anchorDate.ToLocalTime());

        // Tag as local then convert back to UTC
        return DateTime
            .SpecifyKind(nextLocal, DateTimeKind.Local)
            .ToUniversalTime();
    }
}