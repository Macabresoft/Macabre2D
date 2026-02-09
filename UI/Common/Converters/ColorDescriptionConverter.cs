namespace Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Converter for displaying a color as a string.
/// </summary>
public class ColorDescriptionConverter : IValueConverter {
    /// <summary>
    /// Static instance of <see cref="ColorDescriptionConverter" />.
    /// </summary>
    public static readonly ColorDescriptionConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Color color) {
            return $"{color.ToHex()} (R{color.R}, G{color.G}, B{color.B}, A{color.A})";
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return AvaloniaProperty.UnsetValue;
    }
}