namespace Api.ValueTypes;

public readonly record struct JobCompletedPhotoId(Guid Value) : IComparable
{
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