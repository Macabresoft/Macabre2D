namespace Macabresoft.Macabre2D.Common.Attributes;

/// <summary>
/// A reference to a sprite sheet asset's identifiers.
/// </summary>
/// <param name="SpriteSheetId">The content identifier referencing the sprite sheet.</param>
/// <param name="AssetId">The asset identifier that belongs to the sprite sheet.</param>
/// <remarks>
/// This is generic and should only really be used to include sprite sheet assets in project configuration.
/// </remarks>
public readonly record struct SpriteSheetAssetGuidReference(Guid SpriteSheetId, Guid AssetId);