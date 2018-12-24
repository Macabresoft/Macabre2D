namespace Macabre2D.UI.Converters {

    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class ObjectToTypeNameConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value?.GetType().Name ?? "null";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}