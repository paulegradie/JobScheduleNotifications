namespace Api.ValueTypes;

public readonly record struct OrganizationId(Guid Value) : IComparable, IParseString<OrganizationId>
{
    public static OrganizationId Parse(string s)
    {
        if (Guid.TryParse(s, out var g))
            return new OrganizationId(g);
        throw new FormatException($"Cannot convert '{s}' to OrganizationId");
    }

    public override string ToString() => Value.ToString();

    public static OrganizationId New() => new(Guid.NewGuid());
    public static implicit operator Guid(OrganizationId id) => id.Value;
    public static implicit operator OrganizationId(Guid g) => new(g);

    public int CompareTo(OrganizationId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is OrganizationId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}