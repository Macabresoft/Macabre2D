namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

/// <summary>
/// Converts an object to an enumerable. Useful for the root of a tree.
/// </summary>
public class ObjectToEnumerableConverter : IValueConverter {
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value != null ? new[] { value } : Enumerable.Empty<object>();
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is IEnumerable<object> enumerable) {
            return enumerable.FirstOrDefault();
        }

        return null;
    }
}