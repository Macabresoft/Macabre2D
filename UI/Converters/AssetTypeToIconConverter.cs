namespace Macabre2D.UI.Converters {

    using Macabre2D.UI.Models;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class AssetTypeToIconConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var kind = PackIconMaterialKind.File;
            if (value is AssetType assetType) {
                switch (assetType) {
                    case AssetType.Folder:
                        kind = PackIconMaterialKind.Folder;
                        break;

                    case AssetType.Image:
                        kind = PackIconMaterialKind.FileImage;
                        break;

                    case AssetType.Audio:
                        kind = PackIconMaterialKind.FileMusic;
                        break;

                    case AssetType.Scene:
                        kind = PackIconMaterialKind.FileCloud;
                        break;

                    case AssetType.File:
                        kind = PackIconMaterialKind.File;
                        break;

                    case AssetType.Sprite:
                        kind = PackIconMaterialKind.ImageOutline;
                        break;

                    case AssetType.SpriteAnimation:
                        kind = PackIconMaterialKind.FileVideo;
                        break;

                    case AssetType.Font:
                        kind = PackIconMaterialKind.FileDocument;
                        break;
                }
            }

            return kind;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}