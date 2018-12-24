namespace Macabre2D.UI.Converters {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class EqualityToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null && parameter == null) {
                return Visibility.Visible;
            }
            else if (value == null && parameter != null) {
                return Visibility.Collapsed;
            }
            else if (value.Equals(parameter)) {
                return Visibility.Visible;
            }
            else {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}