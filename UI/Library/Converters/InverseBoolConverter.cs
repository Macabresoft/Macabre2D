namespace Macabre2D.UI.Library.Converters {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class InverseBoolConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool boolean) {
                return !boolean;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool boolean) {
                return !boolean;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}