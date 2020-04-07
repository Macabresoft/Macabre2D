namespace Macabre2D.UI.Library.Converters {

    using Macabre2D.UI.Library.Models;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class AssetParentToCollectionConverter : IValueConverter, IMultiValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var collection = new ObservableCollection<IParent<Asset>>();

            var allowNull = parameter is bool boolParameter ? boolParameter : false;
            if (allowNull) {
                collection.Add(new NullAsset());
            }

            if (value is IParent<Asset> parent) {
                collection.Add(parent);
            }

            return collection;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return values.Length > 0 ? this.Convert(values[0], targetType, values.Length > 1 ? values[1] : false, culture) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}