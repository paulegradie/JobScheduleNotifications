namespace Api.ValueTypes;

public readonly record struct RecurrencePatternId(Guid Value) : IComparable, IParseString<RecurrencePatternId>
{
    public static RecurrencePatternId Parse(string s)
    {
        if (!Guid.TryParse(s, out var guid))
            throw new FormatException($"Cannot parse '{s}' as a RecurrencePatternId");
        return new RecurrencePatternId(guid);
    }

    public override string ToString() => Value.ToString();

    public static RecurrencePatternId New() => new(Guid.NewGuid());
    public static implicit operator Guid(RecurrencePatternId id) => id.Value;
    public static implicit operator RecurrencePatternId(Guid g) => new(g);

    public int CompareTo(RecurrencePatternId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is RecurrencePatternId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}