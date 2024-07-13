namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// Converts a <see cref="PredefinedInteger" /> into a display string.
/// </summary>
public class PredefinedIntegerToDisplayConverter : IValueConverter {
    /// <summary>
    /// The static instance of <see cref="PredefinedIntegerToDisplayConverter" />.
    /// </summary>
    public static readonly IValueConverter Instance = new PredefinedIntegerToDisplayConverter();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is PredefinedInteger predefinedInteger) {
            return predefinedInteger.IsUndefined() ? "Undefined" : $"{predefinedInteger.Name} ({predefinedInteger.Value})";
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}