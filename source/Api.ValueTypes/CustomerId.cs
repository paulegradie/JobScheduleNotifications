namespace Api.ValueTypes;

public readonly record struct CustomerId(Guid Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public static CustomerId New() => new(Guid.NewGuid());
    public static implicit operator Guid(CustomerId id) => id.Value;
    public static implicit operator CustomerId(Guid g) => new(g);

    public int CompareTo(CustomerId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is CustomerId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}