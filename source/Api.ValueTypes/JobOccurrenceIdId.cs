namespace Api.ValueTypes;

public readonly record struct JobOccurrenceId(Guid Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public static JobOccurrenceId New() => new(Guid.NewGuid());
    public static implicit operator Guid(JobOccurrenceId id) => id.Value;
    public static implicit operator JobOccurrenceId(Guid g) => new(g);

    public int CompareTo(JobOccurrenceId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is JobOccurrenceId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}