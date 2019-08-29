namespace Macabre2D.UI.Converters {

    using Macabre2D.UI.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public sealed class ComponentCommandsToMenuItemsConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = new List<MenuItem>();
            if (value is IEnumerable<ComponentCommand> commands) {
                foreach (var command in commands) {
                    result.Add(new MenuItem() {
                        Header = command.Name,
                        Command = command.Command
                    });
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}