namespace Macabre2D.UI.CommonLibrary.Converters {

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    public sealed class EnumerableToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = Visibility.Collapsed;
            if (value is IEnumerable enumerable && enumerable.Cast<object>().Any()) {
                result = Visibility.Visible;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}