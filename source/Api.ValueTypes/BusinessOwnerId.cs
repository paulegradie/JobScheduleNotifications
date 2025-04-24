namespace Api.ValueTypes;

public readonly record struct BusinessOwnerId(Guid Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public static BusinessOwnerId New() => new(Guid.NewGuid());
    public static implicit operator Guid(BusinessOwnerId id) => id.Value;
    public static implicit operator BusinessOwnerId(Guid g) => new(g);

    public int CompareTo(BusinessOwnerId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is BusinessOwnerId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}