namespace Macabresoft.Macabre2D.Editor.UI.Converters {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Avalonia.Data.Converters;

    /// <summary>
    /// Converts from a <see cref="Type"/> of an enum to a collection of all the distinct enum values.
    /// </summary>
    public sealed class EnumTypeToValuesConverter : IValueConverter {

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Type enumType && enumType.IsEnum) {
                return Enum.GetValues(enumType);
            }

            return new List<object>();
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}