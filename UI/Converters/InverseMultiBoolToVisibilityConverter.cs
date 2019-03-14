namespace Macabre2D.UI.Converters {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class InverseMultiBoolToVisibilityConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var visibility = Visibility.Visible;
            var multiBoolToVisibilityConverter = new MultiBoolToVisibilityConverter();

            if (multiBoolToVisibilityConverter.Convert(values, targetType, parameter, culture) is Visibility result) {
                visibility = result == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }

            return visibility;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}