namespace Server.Contracts.Cron;

/// <summary>
/// Fluent factory for building CronSchedule instances.
/// </summary>
public class FluentCron
{
    private string _minute = "*";
    private string _hour = "*";
    private string _dayOfMonth = "*";
    private string _month = "*";
    private string _dayOfWeek = "*";

    private FluentCron()
    {
    }

    /// <summary>Create a new FluentCron factory.</summary>
    public static FluentCron Create() => new FluentCron();

    public FluentCron EveryMinute()
    {
        _minute = "*";
        return this;
    }

    public FluentCron AtMinute(int minute)
    {
        if (minute < 0 || minute > 59)
            throw new ArgumentOutOfRangeException(nameof(minute));
        _minute = minute.ToString();
        return this;
    }

    public FluentCron EveryHour()
    {
        _hour = "*";
        return this;
    }

    public FluentCron AtHour(int hour)
    {
        if (hour < 0 || hour > 23)
            throw new ArgumentOutOfRangeException(nameof(hour));
        _hour = hour.ToString();
        return this;
    }

    public FluentCron OnDayOfMonth(int day)
    {
        if (day < 1 || day > 31)
            throw new ArgumentOutOfRangeException(nameof(day));
        _dayOfMonth = day.ToString();
        return this;
    }

    public FluentCron EveryDays(int days)
    {
        if (days is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(days));

        _dayOfMonth = $"*/{days}";
        return this;
    }

    public FluentCron EveryWeeks(int weeks)
    {
        if (weeks < 1) throw new ArgumentOutOfRangeException(nameof(weeks));
        _dayOfMonth = $"*/{weeks * 7}";
        return this;

    }

    public FluentCron InMonth(int month)
    {
        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month));
        _month = month.ToString();
        return this;
    }

    public FluentCron EveryMonths(int months)
    {
        if (months < 1 || months > 12)
            throw new ArgumentOutOfRangeException(nameof(months));
        _month = $"*/{months}";
        return this;
    }

    public FluentCron OnDayOfWeek(int dayOfWeek)
    {
        if (dayOfWeek < 0 || dayOfWeek > 6)
            throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
        _dayOfWeek = dayOfWeek.ToString();
        return this;
    }

    /// <summary>
    /// Build the CronSchedule domain object.
    /// </summary>
    public CronSchedule Build()
        => new CronSchedule(_minute, _hour, _dayOfMonth, _month, _dayOfWeek);
}