namespace Macabre2D.UI.CommonLibrary.Converters {

    using Macabre2D.UI.CommonLibrary.Models;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class AssetItemSourceConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is FolderAsset folderAsset) {
                return folderAsset.Children;
            }
            else {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}