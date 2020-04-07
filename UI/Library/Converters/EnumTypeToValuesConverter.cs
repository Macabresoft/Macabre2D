namespace Macabre2D.UI.Library.Converters {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class EnumTypeToValuesConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Type enumType && enumType.IsEnum) {
                return Enum.GetValues(enumType);
            }

            return new List<object>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}