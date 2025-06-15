namespace Api.ValueTypes;

public readonly record struct JobCompletedPhotoId(Guid Value) : IComparable, IParseString<JobCompletedPhotoId>
{
    public static JobCompletedPhotoId Parse(string s)
    {
        if (Guid.TryParse(s, out var g))
            return new JobCompletedPhotoId(g);
        throw new FormatException($"Cannot convert '{s}' to JobCompletedPhotoId");
    }

    public override string ToString() => Value.ToString();

    public static JobCompletedPhotoId New() => new(Guid.NewGuid());
    public static implicit operator Guid(JobCompletedPhotoId id) => id.Value;
    public static implicit operator JobCompletedPhotoId(Guid g) => new(g);

    public int CompareTo(JobCompletedPhotoId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is JobCompletedPhotoId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}