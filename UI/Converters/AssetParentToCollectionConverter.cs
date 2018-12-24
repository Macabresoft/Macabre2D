namespace Macabre2D.UI.Converters {

    using Macabre2D.UI.Models;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class AssetParentToCollectionConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var collection = new ObservableCollection<IParent<Asset>>();
            if (value is IParent<Asset> parent) {
                collection.Add(parent);
            }

            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}