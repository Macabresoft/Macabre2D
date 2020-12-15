namespace Macabresoft.Macabre2D.Editor.UI.Converters {
    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Data.Converters;
    using Macabresoft.Macabre2D.Editor.Library.Models;

    public class EntitySelectionKindToBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = false;
            if (value is EntitySelectionKind valueKind && parameter is EntitySelectionKind parameterKind) {
                result = valueKind == parameterKind;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is bool boolValue && boolValue ? parameter : AvaloniaProperty.UnsetValue;
        }
    }
}