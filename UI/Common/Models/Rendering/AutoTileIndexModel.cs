namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A model for a single tile of an <see cref="AutoTileSet" />.
/// </summary>
public class AutoTileIndexModel : PropertyChangedNotifier {
    private readonly AutoTileSet _tileSet;
    private byte? _spriteIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTileIndexModel" /> class.
    /// </summary>
    /// <param name="tileSet">The tile set.</param>
    /// <param name="tileIndex">The tile index.</param>
    public AutoTileIndexModel(AutoTileSet tileSet, byte tileIndex) {
        this._tileSet = tileSet;
        this.TileIndex = tileIndex;

        if (this._tileSet.TryGetSpriteIndex(tileIndex, out var spriteIndex)) {
            this._spriteIndex = spriteIndex;
        }
    }

    /// <summary>
    /// Gets the tile index.
    /// </summary>
    public byte TileIndex { get; }

    /// <summary>
    /// Gets or sets the sprite index.
    /// </summary>
    public byte? SpriteIndex {
        get => this._spriteIndex;
        set {
            if (this.Set(ref this._spriteIndex, value)) {
                if (this._spriteIndex == null) {
                    this._tileSet.UnsetSprite(this.TileIndex);
                }
                else {
                    this._tileSet.SetSprite(this._spriteIndex.Value, this.TileIndex);
                }
            }
        }
    }
}