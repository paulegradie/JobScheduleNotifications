using System.ComponentModel;
using System.Globalization;

namespace Api.ValueTypes;

// [TypeConverter(typeof(CustomerIdTypeConverter))]
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

public class CustomerIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s && Guid.TryParse(s, out var g))
            return new CustomerId(g);
        throw new FormatException($"Cannot convert '{value}' to CustomerId");
    }
}