namespace Macabre2D.Engine.Windows.Converters {

    using Macabre2D.Engine.Windows.Models.FrameworkWrappers;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class ComponentWrapperEditingStyleToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = Visibility.Collapsed;

            if (value is ComponentWrapper component && parameter is ComponentEditingStyle editingStyle) {
                result = (component.EditingStyle & editingStyle) == editingStyle ? Visibility.Visible : Visibility.Collapsed;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}