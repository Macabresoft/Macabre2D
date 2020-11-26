namespace Macabresoft.Macabre2D.Editor.UI.Converters {
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;

    /// <summary>
    /// A converter that takes two objects and returns a value indicating whether or not they are equal.
    /// </summary>
    public class EqualityToBoolConverter : IValueConverter {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == parameter;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return parameter;
        }
    }
}