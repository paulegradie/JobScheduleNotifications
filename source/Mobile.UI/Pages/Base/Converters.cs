using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Mobile.UI.Pages.Base;

/// <summary>
/// If value != null, returns string.Format(format,value); otherwise returns DefaultText.
/// </summary>
public class NullToDefaultConverter : IValueConverter
{
    public string DefaultText { get; set; } = "";
    public string Format      { get; set; } = "{0}";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // parameter can override DefaultText or Format
        if (value == null)
            return parameter as string ?? DefaultText;

        if (value is IFormattable f)
            return string.Format(culture, parameter as string ?? Format, f);
            
        return value.ToString()!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Returns true if value != null, false otherwise.
/// </summary>
public class NotNullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value != null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}