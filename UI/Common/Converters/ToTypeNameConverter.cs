namespace Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

/// <summary>
/// Converts from a <see cref="object" /> to a type display name.
/// </summary>
public class ToTypeNameConverter : IValueConverter {
    private enum TypeNameConversion {
        DisplayName,
        WithNameSpace,
        Normal
    }

    private readonly TypeNameConversion _conversion;

    private ToTypeNameConverter(TypeNameConversion conversion) {
        this._conversion = conversion;
    }
    
    /// <summary>
    /// A static instance of this converter that gives the type's display name.
    /// </summary>
    public static readonly IValueConverter DisplayName = new ToTypeNameConverter(TypeNameConversion.DisplayName);
    
    /// <summary>
    /// A static instance of this converter that gives the full type name with namespace.
    /// </summary>
    public static readonly IValueConverter FullName = new ToTypeNameConverter(TypeNameConversion.WithNameSpace);
    
    /// <summary>
    /// A static instance of this converter that gives the type name.
    /// </summary>
    public static readonly IValueConverter Name = new ToTypeNameConverter(TypeNameConversion.Normal);

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value != null) {
            return this._conversion switch {
                TypeNameConversion.Normal => value.GetType().Name,
                TypeNameConversion.WithNameSpace => value.GetType().FullName,
                TypeNameConversion.DisplayName => ToDisplayNameConverter.Instance.Convert(value.GetType(), targetType, parameter, culture),
                _ => throw new NotSupportedException()
            };
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}