namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

/// <summary>
/// Converts a guid to a value indicating whether the guid is not empty.
/// </summary>
public class GuidToIsNotEmptyConverter : IValueConverter {

    /// <summary>
    /// Static instance of <see cref="GuidToIsNotEmptyConverter" />.
    /// </summary>
    public static readonly IValueConverter Instance = new GuidToIsNotEmptyConverter();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Guid id) {
            return id != Guid.Empty;
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}