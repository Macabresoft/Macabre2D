namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

/// <summary>
/// Converts from a <see cref="object" /> to a type display name.
/// </summary>
public class ToTypeNameConverter : IValueConverter {

    /// <summary>
    /// A static instance of this converter.
    /// </summary>
    public static readonly ToTypeNameConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? ToDisplayNameConverter.Instance.Convert(value.GetType(), targetType, parameter, culture) : AvaloniaProperty.UnsetValue;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}