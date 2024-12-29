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
    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static IValueConverter Instance { get; } = new AssetToIconConverter();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var result = AvaloniaProperty.UnsetValue;

        if (Application.Current is { } application) {
            var actualValue = value;

            if (value is FilteredContentWrapper wrapper) {
                actualValue = wrapper.Node;
            }

            _ = actualValue switch {
                ContentFile { Asset: { } asset } => asset switch {
                    AudioClip => application.TryFindResource("AudioClipIcon", out result),
                    PrefabAsset => application.TryFindResource("EntityIcon", out result),
                    SceneAsset => application.TryFindResource("SceneIcon", out result),
                    ShaderAsset => application.TryFindResource("ShaderIcon", out result),
                    SpriteSheet => application.TryFindResource("SpriteSheetIcon", out result),
                    _ => application.TryFindResource("FileIcon", out result)
                },
                INameableCollection => actualValue switch {
                    AutoTileSetCollection => application.TryFindResource("AutoLayoutIcon", out result),
                    SpriteAnimationCollection => application.TryFindResource("AnimationIcon", out result),
                    SpriteSheetFontCollection => application.TryFindResource("FontIcon", out result),
                    SpriteSheetIconSetCollection => application.TryGetResource("IconSetIcon", out result),
                    SpriteAnimationSetCollection => application.TryFindResource("AnimationIcon", out result),
                    _ => application.TryFindResource("UnknownIcon", out result)
                },
                _ => application.TryFindResource("UnknownIcon", out result)
            };
        }

        return result;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}