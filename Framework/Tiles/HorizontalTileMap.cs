namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;

/// <summary>
/// An entity which maps an <see cref="AutoTileSet" /> onto a grid, but only connects horizontally.
/// </summary>
[Display(Name = "Horizontal Tile Map")]
public sealed class HorizontalTileMap : BaseAutoTileMap {
    /// <inheritdoc />
    protected override byte GetIndex(Point tile) {
        var direction = CardinalDirections.None;

        if (this.HasActiveTileAt(tile + new Point(1, 0))) {
            direction |= CardinalDirections.East;
        }

        if (this.HasActiveTileAt(tile - new Point(1, 0))) {
            direction |= CardinalDirections.West;
        }

        return (byte)direction;
    }
}