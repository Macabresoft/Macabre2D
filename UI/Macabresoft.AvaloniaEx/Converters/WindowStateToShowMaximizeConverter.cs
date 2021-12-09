namespace Macabresoft.AvaloniaEx;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

public class WindowStateToShowMaximizeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is WindowState windowState) {
            var result = windowState is WindowState.Normal or WindowState.Minimized;
            return parameter is true ? !result : result;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}