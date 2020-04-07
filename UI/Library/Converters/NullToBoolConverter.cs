namespace Macabre2D.UI.Library.Converters {

    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class NullToBoolConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}