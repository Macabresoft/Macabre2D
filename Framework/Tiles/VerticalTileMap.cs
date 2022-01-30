namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;

/// <summary>
/// An entity which maps an <see cref="AutoTileSet" /> onto a grid, but only connects vertically.
/// </summary>
[Display(Name = "Vertical Tile Map")]
public sealed class VerticalTileMap : BaseAutoTileMap {
    /// <inheritdoc />
    protected override byte GetIndex(Point tile) {
        var direction = CardinalDirections.None;
        if (this.HasActiveTileAt(tile + new Point(0, 1))) {
            direction |= CardinalDirections.North;
        }

        if (this.HasActiveTileAt(tile - new Point(0, 1))) {
            direction |= CardinalDirections.South;
        }

        return (byte)direction;
    }
}