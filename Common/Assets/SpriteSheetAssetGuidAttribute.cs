namespace Macabresoft.Macabre2D.Common.Attributes;

/// <summary>
/// Enum representing a kind of spritesheet when there is no ability to reference the <see cref="Type"/>.
/// </summary>
public enum SpriteSheetAssetKind {
    Animation,
    AutoTileSet,
    Font,
    GamePadIconSet,
    KeyboardIconSet
}

/// <summary>
/// Attribute for <see cref="SpriteSheetAssetGuidReference" /> to reference a specific <see cref="SpriteSheetAssetKind"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SpriteSheetAssetGuidAttribute : Attribute {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetAssetGuidAttribute" /> class.
    /// </summary>
    /// <param name="assetKind">The asset kind.</param>
    public SpriteSheetAssetGuidAttribute(SpriteSheetAssetKind assetKind) {
        this.AssetKind = assetKind;
    }
    
    /// <summary>
    /// Gets the asset kind.
    /// </summary>
    public SpriteSheetAssetKind AssetKind { get; }
}