namespace Api.ValueTypes;

public readonly record struct JobReminderId(Guid Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public static JobReminderId New() => new(Guid.NewGuid());
    public static implicit operator Guid(JobReminderId id) => id.Value;
    public static implicit operator JobReminderId(Guid g) => new(g);

    public int CompareTo(JobReminderId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is JobReminderId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}