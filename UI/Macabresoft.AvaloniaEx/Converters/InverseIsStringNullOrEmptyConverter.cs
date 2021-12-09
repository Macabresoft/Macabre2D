namespace Macabresoft.AvaloniaEx;

using System;
using System.Globalization;
using Avalonia.Data.Converters;

/// <summary>
/// Converts a string to a bool. The result will be true if the string is not null or empty.
/// </summary>
public class InverseIsStringNullOrEmptyConverter : IValueConverter {
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value is string stringValue && !string.IsNullOrEmpty(stringValue);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}