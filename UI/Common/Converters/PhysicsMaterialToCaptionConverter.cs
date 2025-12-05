namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// Converts from a <see cref="PhysicsMaterial" /> to a caption.
/// </summary>
public class PhysicsMaterialToCaptionConverter : IValueConverter {
    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static IValueConverter Instance { get; } = new PhysicsMaterialToCaptionConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is PhysicsMaterial material) {
            return $"Bounce: {material.Bounce:F2}, Friction {material.Friction:F2}";
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}