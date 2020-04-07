namespace Macabre2D.UI.Library.Converters {

    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class EqualityToBoolConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value == null && parameter == null) || (value != null && value.Equals(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}