namespace Macabresoft.AvaloniaEx;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

/// <summary>
/// A converter which checks if an object is of the provided type.
/// </summary>
public class IsTypeConverter : IValueConverter {
    /// <summary>
    /// Gets or sets a value indicating whether or not the result should be inverted.
    /// </summary>
    public bool InvertResult { get; set; }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value != null && parameter is Type type) {
            var result = value.GetType().IsAssignableTo(type);
            return this.InvertResult ? !result : result;
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}