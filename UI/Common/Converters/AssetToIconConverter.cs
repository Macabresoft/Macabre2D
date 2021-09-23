namespace Macabresoft.Macabre2D.UI.Common.Converters {
    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data.Converters;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    /// <summary>
    /// Converts from an asset to an icon.
    /// </summary>
    public class AssetToIconConverter : IValueConverter {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            object result = null;

            if (value is ContentFile { Asset: IAsset asset }) {
                _ = asset switch {
                    SceneAsset => Application.Current.TryFindResource("SceneAssetIcon", out result),
                    SpriteSheet => Application.Current.TryFindResource("SpriteSheetIcon", out result),
                    AudioClip => Application.Current.TryFindResource("AudioClipIcon", out result),
                    _ => Application.Current.TryFindResource("FileIcon", out result)
                };
            }

            if (result == null) {
                Application.Current.TryFindResource("FileIcon", out result);
            }

            return result;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}