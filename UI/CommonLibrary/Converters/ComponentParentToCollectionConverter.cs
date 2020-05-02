namespace Macabre2D.UI.CommonLibrary.Converters {

    using Macabre2D.Framework;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class ComponentParentToCollectionConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var collection = new ObservableCollection<Scene>();
            if (value is Scene parent) {
                collection.Add(parent);
            }

            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}