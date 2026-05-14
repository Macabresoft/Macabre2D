namespace Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Macabre2D.Project.Common;

/// <summary>
/// Converts from a <see cref="ResourceCulture" /> to its display name.
/// </summary>
public class ResourceCultureToDisplayNameConverter : IValueConverter {
    /// <summary>
    /// A static instance of this converter.
    /// </summary>
    public static readonly ResourceCultureToDisplayNameConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is ResourceCulture resourceCulture) {
            return resourceCulture.ToCultureName();
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}