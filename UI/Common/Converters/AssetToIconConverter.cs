namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// Converts from an asset to an icon.
/// </summary>
public class AssetToIconConverter : IValueConverter {
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        object result;

        _ = value switch {
            ContentFile { Asset: IAsset asset } => asset switch {
                SceneAsset => Application.Current.TryFindResource("SceneIcon", out result),
                SpriteSheet => Application.Current.TryFindResource("SpriteSheetIcon", out result),
                AudioClip => Application.Current.TryFindResource("AudioClipIcon", out result),
                Shader => Application.Current.TryFindResource("ShaderIcon", out result),
                _ => Application.Current.TryFindResource("FileIcon", out result)
            },
            INameableCollection => value switch {
                AutoTileSetCollection => Application.Current.TryFindResource("AutoLayoutIcon", out result),
                SpriteAnimationCollection => Application.Current.TryFindResource("AnimationIcon", out result),
                _ => Application.Current.TryFindResource("UnknownIcon", out result)
            },
            _ => Application.Current.TryFindResource("UnknownIcon", out result)
        };

        return result;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}