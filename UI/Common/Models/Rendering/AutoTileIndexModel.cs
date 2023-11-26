namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A model for a single tile of an <see cref="AutoTileSet" />.
/// </summary>
public class AutoTileIndexModel : BaseSpriteSheetIndexModel<AutoTileSet, byte> {
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTileIndexModel" /> class.
    /// </summary>
    /// <param name="tileSet">The tile set.</param>
    /// <param name="tileIndex">The tile index.</param>
    public AutoTileIndexModel(AutoTileSet tileSet, byte tileIndex) : base(tileSet, tileIndex) {
        if (this.Member.TryGetSpriteIndex(tileIndex, out var spriteIndex)) {
            this.SpriteIndex = spriteIndex;
        }
    }
}