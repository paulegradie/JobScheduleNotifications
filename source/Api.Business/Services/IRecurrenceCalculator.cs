
using Api.Business.Entities;

namespace Api.Business.Services;

public interface IRecurrenceCalculator
{
    /// <summary>
    /// Given a recurrence pattern, its anchor date, and the last occurrence,
    /// return the next DateTime (in UTC).
    /// </summary>
    DateTime GetNextOccurrence(
        RecurrencePatternDomainModel pattern,
        DateTime anchorDateUtc,
        DateTime afterUtc);
}