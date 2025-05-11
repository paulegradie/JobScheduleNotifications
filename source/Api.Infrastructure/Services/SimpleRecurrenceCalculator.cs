using Api.Business.Entities;
using Api.Business.Services;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes.Enums;
using NCrontab;

namespace Api.Infrastructure.Services;

public class SimpleRecurrenceCalculator : IRecurrenceCalculator
{
    public DateTime GetNextOccurrence(
        RecurrencePatternDomainModel pat,
        DateTime anchorDateUtc,
        DateTime afterUtc)
    {
        return pat.Frequency switch
        {
            Frequency.Daily => NextDaily(pat, afterUtc),
            Frequency.Weekly => NextWeekly(pat, anchorDateUtc, afterUtc),
            Frequency.Monthly => NextMonthly(pat, afterUtc),
            // Frequency.Yearly => NextYearly(pat, afterUtc),
            // Frequency.Cron => NextCron(pat, afterUtc),
            _ => throw new NotSupportedException(
                $"Unsupported frequency: {pat.Frequency}")
        };
    }

    private DateTime NextDaily(RecurrencePatternDomainModel pat, DateTime afterUtc)
        => afterUtc
            .Date
            .AddDays(pat.Interval)
            .Add(afterUtc.TimeOfDay);

    private DateTime NextWeekly(
        RecurrencePatternDomainModel pat,
        DateTime anchorDateUtc,
        DateTime afterUtc)
    {
        // scan forward one day at a time until we hit “correct week-interval” + “correct weekday”
        var candidate = afterUtc.Date.AddDays(1);
        while (true)
        {
            var weeksSinceAnchor =
                (int)((candidate - anchorDateUtc.Date).TotalDays / 7);
            var inCorrectInterval = (weeksSinceAnchor % pat.Interval) == 0;

            // map DayOfWeek → WeekDays mask
            var dowFlag = (WeekDay)(1 << (int)candidate.DayOfWeek);
            var isCorrectDay = pat.WeekDays.Any(x => x.HasFlag(dowFlag));

            if (inCorrectInterval && isCorrectDay)
                return candidate.Add(afterUtc.TimeOfDay);

            candidate = candidate.AddDays(1);
        }
    }

    private DateTime NextMonthly(RecurrencePatternDomainModel pat, DateTime afterUtc)
    {
        if (!pat.DayOfMonth.HasValue)
            throw new ArgumentException(
                "DayOfMonth required for monthly recurrence",
                nameof(pat));

        var month = afterUtc.Month + pat.Interval;
        var year = afterUtc.Year + (month - 1) / 12;
        month = ((month - 1) % 12) + 1;

        return new DateTime(
            year,
            month,
            pat.DayOfMonth.Value,
            afterUtc.Hour,
            afterUtc.Minute,
            afterUtc.Second,
            DateTimeKind.Utc);
    }

    private DateTime NextYearly(RecurrencePatternDomainModel pat, DateTime afterUtc)
        => afterUtc
            .AddYears(pat.Interval);

    private DateTime NextCron(RecurrencePatternDomainModel pat, DateTime afterUtc)
    {
        if (string.IsNullOrWhiteSpace(pat.CronExpression))
            throw new ArgumentException(
                "CronExpression is required for Cron frequency",
                nameof(pat));

        // Using NCrontab:
        var schedule = CrontabSchedule.Parse(
            pat.CronExpression,
            new CrontabSchedule.ParseOptions { IncludingSeconds = true });

        var next = schedule.GetNextOccurrence(afterUtc.ToLocalTime());
        // NCrontab returns a LOCAL time, so convert back:
        return DateTime.SpecifyKind(next, DateTimeKind.Local)
            .ToUniversalTime();

        /* If you prefer Cronos, swap in:
        var expr = CronExpression.Parse(pat.CronExpression, CronFormat.IncludeSeconds);
        var nextUtc = expr.GetNextOccurrence(afterUtc, TimeZoneInfo.Utc);
        return nextUtc?.UtcDateTime
               ?? throw new InvalidOperationException("No next cron occurrence");
        */
    }
}