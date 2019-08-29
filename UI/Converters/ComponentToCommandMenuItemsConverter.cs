namespace Macabre2D.UI.Converters {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Data;

    public sealed class ComponentToCommandMenuItemsConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            IReadOnlyCollection<MenuItem> result;
            if (value is ComponentWrapper componentWrapper && componentWrapper.Component != null) {
                result = GetMenuItems(componentWrapper.Component);
            }
            else {
                result = new List<MenuItem>();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        private static IReadOnlyCollection<MenuItem> GetMenuItems(BaseComponent component) {
            // TODO: Is it even possible to worry about undo/redo?
            var menuItems = new List<MenuItem>();
            var methodInfos = component.GetType().GetAllMethods(typeof(ComponentCommandAttribute));

            foreach (var methodInfo in methodInfos) {
                if (methodInfo.GetCustomAttributes(typeof(ComponentCommandAttribute), true).FirstOrDefault() is ComponentCommandAttribute attribute) {
                    var menuItem = new MenuItem() {
                        Header = attribute.Name,
                        Command = new RelayCommand(() => methodInfo.Invoke(component, null), true)
                    };

                    menuItems.Add(menuItem);
                }
            }

            return menuItems;
        }
    }
}