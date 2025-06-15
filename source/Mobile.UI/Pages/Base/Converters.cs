using System.Globalization;

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

/// <summary>
/// Converts a boolean validation state to a background color (white for valid, light red for invalid).
/// </summary>
public class ValidationToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isValid)
        {
            return isValid ? Colors.White : Color.FromArgb("#FFEBEE"); // Light red for invalid
        }
        return Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converts a string to boolean (true if not null/empty, false otherwise).
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}