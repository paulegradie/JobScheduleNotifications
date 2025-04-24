namespace Api.ValueTypes;

public readonly record struct UserId(Guid Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public static UserId New() => new(Guid.NewGuid());
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid g) => new(g);

    public int CompareTo(UserId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is UserId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}