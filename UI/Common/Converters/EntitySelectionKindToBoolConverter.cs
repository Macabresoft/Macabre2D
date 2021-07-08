namespace Macabresoft.Macabre2D.UI.Common.Converters {
    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Data.Converters;
    using Macabresoft.Macabre2D.UI.Common.Models;

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