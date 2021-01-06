namespace Macabresoft.Macabre2D.Editor.UI.Converters {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Avalonia.Data.Converters;

    /// <summary>
    /// Converts from a <see cref="Type"/> of an enum to a collection of all the distinct enum values.
    /// </summary>
    public sealed class EnumTypeToValuesConverter : IValueConverter {

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = new List<object>();
            
            if (value is Type enumType && enumType.IsEnum) {
                var values = Enum.GetValues(enumType);
                var intValues = values.Cast<Enum>().Select(System.Convert.ToInt32).ToList();
                
                for (var i = 0; i < values.Length; i++) {
                    if (intValues[i] != 0) {
                        result.Add(values.GetValue(i));
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}