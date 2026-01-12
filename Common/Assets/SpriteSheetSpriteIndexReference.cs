namespace Macabresoft.Macabre2D.Common;

/// <summary>
/// A reference to a sprite sheet and a sprite index.
/// </summary>
/// <param name="SpriteSheetId">The content identifier referencing the sprite sheet.</param>
/// <param name="SpriteIndex">The sprite index.</param>
/// <remarks>
/// This is generic and should only really be used to include sprites in project configuration.
/// </remarks>
public readonly record struct SpriteSheetSpriteIndexReference(Guid SpriteSheetId, byte SpriteIndex) {
    /// <summary>
    /// Gets an empty instance.
    /// </summary>
    public static SpriteSheetSpriteIndexReference Empty { get; } = new(Guid.Empty, 0);
}