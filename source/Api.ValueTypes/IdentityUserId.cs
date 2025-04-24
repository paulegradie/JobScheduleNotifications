namespace Api.ValueTypes;

public readonly record struct IdentityUserId(Guid Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public static IdentityUserId New() => new(Guid.NewGuid());
    public static implicit operator Guid(IdentityUserId id) => id.Value;
    public static implicit operator IdentityUserId(Guid g) => new(g);

    public int CompareTo(IdentityUserId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is IdentityUserId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}