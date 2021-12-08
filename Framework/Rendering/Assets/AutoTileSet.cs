namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// A tile set which automatically provides the correct sprite given its relationship to surrounding tiles.
/// </summary>
public sealed class AutoTileSet : SpriteSheetAsset {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Tile Set";

    private const byte CardinalSize = 16;

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<byte, byte> _tileIndexToSpriteIndex = new();

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public int Size => CardinalSize;

    /// <summary>
    /// Sets the sprite at the specified index.
    /// </summary>
    /// <param name="spriteIndex">The sprite.</param>
    /// <param name="tileIndex">The index.</param>
    public void SetSprite(byte spriteIndex, byte tileIndex) {
        if (tileIndex < this.Size) {
            this._tileIndexToSpriteIndex[tileIndex] = spriteIndex;
            this.RaisePropertyChanged(nameof(AutoTileSet));
        }
    }

    /// <summary>
    /// Sets the sprite for the tile with the connected directions specified.
    /// </summary>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="connectedDirections">The directions in which this tile connects to other tiles.</param>
    public void SetSprite(byte spriteIndex, CardinalDirections connectedDirections) {
        this.SetSprite(spriteIndex, (byte)connectedDirections);
    }

    /// <summary>
    /// Tries to get the sprite index used by the specified tile.
    /// </summary>
    /// <param name="tileIndex">The tile index.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <returns>A value indicating whether or not the sprite was found.</returns>
    public bool TryGetSpriteIndex(byte tileIndex, out byte spriteIndex) {
        return this._tileIndexToSpriteIndex.TryGetValue(tileIndex, out spriteIndex);
    }

    /// <summary>
    /// Removes the sprite index for the given tile index, effectively blanking it out.
    /// </summary>
    /// <param name="tileIndex">The tile index.</param>
    public void UnsetSprite(byte tileIndex) {
        this._tileIndexToSpriteIndex.Remove(tileIndex);
        this.RaisePropertyChanged(nameof(AutoTileSet));
    }
}