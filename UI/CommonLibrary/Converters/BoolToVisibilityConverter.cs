namespace Macabre2D.UI.CommonLibrary.Converters {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class BoolToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = Visibility.Collapsed;

            if (value is bool boolValue) {
                result = boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}