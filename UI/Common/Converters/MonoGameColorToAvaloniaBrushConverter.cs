namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Color = Microsoft.Xna.Framework.Color;

public class MonoGameColorToAvaloniaBrushConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Color color) {
            return new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B));
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is SolidColorBrush brush) {
            return new Color(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A);
        }

        return AvaloniaProperty.UnsetValue;
    }
}