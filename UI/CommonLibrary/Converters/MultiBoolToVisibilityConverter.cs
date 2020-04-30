namespace Macabre2D.UI.CommonLibrary.Converters {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class MultiBoolToVisibilityConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var visible = true;
            foreach (var value in values) {
                if (value is bool boolValue) {
                    visible = visible && boolValue;
                }
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}