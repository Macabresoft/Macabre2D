namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;

/// <summary>
/// An entity which maps an <see cref="AutoTileSet" /> onto a grid.
/// </summary>
[Display(Name = "Auto Tile Map")]
public sealed class AutoTileMap : BaseAutoTileMap {
    /// <inheritdoc />
    protected override byte GetIndex(Point tile) {
        var direction = CardinalDirections.None;
        if (this.HasActiveTileAt(tile + new Point(0, 1))) {
            direction |= CardinalDirections.North;
        }

        if (this.HasActiveTileAt(tile + new Point(1, 0))) {
            direction |= CardinalDirections.East;
        }

        if (this.HasActiveTileAt(tile - new Point(0, 1))) {
            direction |= CardinalDirections.South;
        }

        if (this.HasActiveTileAt(tile - new Point(1, 0))) {
            direction |= CardinalDirections.West;
        }

        return (byte)direction;
    }
}