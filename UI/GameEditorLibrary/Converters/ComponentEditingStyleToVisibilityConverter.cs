namespace Macabre2D.UI.GameEditorLibrary.Converters {

    using Macabre2D.Framework;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class ComponentEditingStyleToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = Visibility.Collapsed;

            if (value is BaseComponent component && parameter is ComponentEditingStyle editingStyle) {
                result = (component.GetEditingStyle() & editingStyle) == editingStyle ? Visibility.Visible : Visibility.Collapsed;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}