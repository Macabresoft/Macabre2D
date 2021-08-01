namespace Macabresoft.Macabre2D.UI.Common.Converters {
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;

    /// <summary>
    /// Takes a value and a parameter. If these two things are equal, returns true.
    /// </summary>
    public class EqualityConverter : IValueConverter {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != null && value.Equals(parameter);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is true ? parameter : null;
        }
    }
}