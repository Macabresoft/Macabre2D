namespace Macabre2D.UI.Library.Converters {

    using Macabre2D.Framework;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    public sealed class ComponentHighlightColorConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var result = DependencyProperty.UnsetValue;

            if (values.Length == 4 && values[0] is Type highlightType && values[2] is Brush highlightColor && values[3] is Brush nonHighlightColor) {
                if (values[1] is BaseComponent component) {
                    result = highlightType.IsAssignableFrom(component.GetType()) ? highlightColor : nonHighlightColor;
                }
                else {
                    result = nonHighlightColor;
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}