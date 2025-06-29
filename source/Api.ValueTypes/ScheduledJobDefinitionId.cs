﻿using System.ComponentModel;
using System.Globalization;

namespace Api.ValueTypes;

public readonly record struct ScheduledJobDefinitionId(Guid Value) : IComparable ,IParseString<ScheduledJobDefinitionId>
{
    public static ScheduledJobDefinitionId Parse(string s)
    {
        if (!Guid.TryParse(s, out var g))
            throw new FormatException($"Cannot parse '{s}' as a Guid");
        return new ScheduledJobDefinitionId(g);
    }

    public override string ToString() => Value.ToString();

    public static ScheduledJobDefinitionId New() => new(Guid.NewGuid());
    public static implicit operator Guid(ScheduledJobDefinitionId id) => id.Value;
    public static implicit operator ScheduledJobDefinitionId(Guid g) => new(g);

    public int CompareTo(ScheduledJobDefinitionId other)
        => Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj is ScheduledJobDefinitionId other
            ? CompareTo(other)
            : throw new ArgumentException(
                $"Cannot compare UserId to {obj?.GetType().Name}", nameof(obj));
}

public class ScheduledJobDefinitionIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s && Guid.TryParse(s, out var g))
            return new ScheduledJobDefinitionId(g);
        throw new FormatException($"Cannot convert '{value}' to CustomerId");
    }
}