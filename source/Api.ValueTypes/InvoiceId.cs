using System.ComponentModel;
using System.Globalization;

namespace Api.ValueTypes;

/// 
public readonly record struct InvoiceId(Guid Value) : IComparable, IParseString<InvoiceId>
{
    public static InvoiceId Parse(string s)
    {
        if (Guid.TryParse(s, out var g))
            return new InvoiceId(g);
        throw new FormatException($"Cannot convert '{s}' to InvoiceId");
    }

    public override string ToString() => Value.ToString();

    public static InvoiceId New() => new(Guid.NewGuid());
    public static implicit operator Guid(InvoiceId id) => id.Value;
    public static implicit operator InvoiceId(Guid g) => new(g);

    public int CompareTo(InvoiceId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is InvoiceId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}

public class InvoiceIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s && Guid.TryParse(s, out var g))
            return new InvoiceId(g);
        throw new FormatException($"Cannot convert '{value}' to InvoiceId");
    }
}