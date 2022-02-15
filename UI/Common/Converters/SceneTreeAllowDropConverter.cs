namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// This is poorly named, but it converts from a node's DataContext to a value indicating whether or not a drop is valid.
/// </summary>
public class SceneTreeAllowDropConverter : IValueConverter {
    /// <summary>
    /// A singleton instance.
    /// </summary>
    public static SceneTreeAllowDropConverter Instance { get; } = new();
    
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value is IEntity or EntityCollection;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}