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
        var result = AvaloniaProperty.UnsetValue;

        if (Application.Current is { } application) {
            _ = value switch {
                ContentFile { Asset: { } asset } => asset switch {
                    AudioClipAsset => application.TryFindResource("AudioClipIcon", out result),
                    PrefabAsset => application.TryFindResource("EntityIcon", out result),
                    SceneAsset => application.TryFindResource("SceneIcon", out result),
                    ShaderAsset => application.TryFindResource("ShaderIcon", out result),
                    SpriteSheetAsset => application.TryFindResource("SpriteSheetIcon", out result),
                    _ => application.TryFindResource("FileIcon", out result)
                },
                INameableCollection => value switch {
                    AutoTileSetCollection => application.TryFindResource("AutoLayoutIcon", out result),
                    SpriteAnimationCollection => application.TryFindResource("AnimationIcon", out result),
                    SpriteSheetFontCollection => application.TryFindResource("FontIcon", out result),
                    _ => application.TryFindResource("UnknownIcon", out result)
                },
                _ => application.TryFindResource("UnknownIcon", out result)
            };
        }

        return result;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}