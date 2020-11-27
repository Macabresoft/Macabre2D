namespace Macabresoft.Macabre2D.Editor.UI.Converters {
    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Data.Converters;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame;

    /// <summary>
    /// A converter that takes two objects and returns a value indicating whether or not they are equal.
    /// </summary>
    public class GizmoKindToBoolConverter : IValueConverter {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = false;
            if (value is GizmoKind valueKind && parameter is GizmoKind parameterKind) {
                result = valueKind == parameterKind;
            }
            
            return result;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is bool boolValue && boolValue ? parameter : AvaloniaProperty.UnsetValue;
        }
    }
}