namespace Macabre2D.UI.Converters {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class AssetTypeToIconConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is AssetType assetType) {
                switch (assetType) {
                    case AssetType.Folder:
                        return IconHelper.GetFolderIcon();

                    case AssetType.Image:
                        return IconHelper.GetFileImageIcon();

                    case AssetType.Audio:
                        return IconHelper.GetFileAudioIcon();

                    case AssetType.Scene:
                        return IconHelper.GetFileSceneIcon();

                    case AssetType.File:
                        return IconHelper.GetFileIcon();

                    case AssetType.Sprite:
                        return IconHelper.GetSpriteIcon();

                    case AssetType.SpriteAnimation:
                        return IconHelper.GetFileSpriteAnimationIcon();

                    case AssetType.Font:
                        return IconHelper.GetFontIcon();
                }
            }

            return IconHelper.GetFileIcon();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}