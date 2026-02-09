namespace Macabre2D.Framework;

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// A tile set which automatically provides the correct sprite given its relationship to surrounding tiles.
/// </summary>
public sealed class AutoTileSet : SpriteSheetKeyedMember<byte> {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Tile Set";

    private const byte CardinalSize = 16;

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<byte, byte> _tileIndexToSpriteIndex = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTileSet" /> class.
    /// </summary>
    public AutoTileSet() : base() {
        this.Name = DefaultName;
    }

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public int Size => CardinalSize;

    /// <summary>
    /// Removes the sprite index for the given tile index, effectively blanking it out.
    /// </summary>
    /// <param name="tileIndex">The tile index.</param>
    public override void ClearSprite(byte tileIndex) {
        this._tileIndexToSpriteIndex.Remove(tileIndex);
        this.RaisePropertyChanged(nameof(AutoTileSet));
    }

    /// <summary>
    /// Sets the sprite at the specified index.
    /// </summary>
    /// <param name="spriteIndex">The sprite.</param>
    /// <param name="tileIndex">The index.</param>
    public override void SetSprite(byte spriteIndex, byte tileIndex) {
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
    /// <returns>A value indicating whether the sprite was found.</returns>
    public bool TryGetSpriteIndex(byte tileIndex, out byte spriteIndex) {
        return this._tileIndexToSpriteIndex.TryGetValue(tileIndex, out spriteIndex);
    }
}