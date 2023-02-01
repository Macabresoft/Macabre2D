namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// An interface for an entity which contains a grid.
/// </summary>
public interface IGridContainer : IEntity {
    /// <summary>
    /// Gets or sets the tile size in local space, unaffected by the scale of this <see cref="ITransformable" />.
    /// </summary>
    Vector2 TileSize { get; set; }

    /// <summary>
    /// Gets the nearest tile position to the provided position.
    /// </summary>
    /// <param name="position">The position of which to find the nearest tile.</param>
    /// <returns>The nearest tile position in world space.</returns>
    Vector2 GetNearestTilePosition(Vector2 position);

    /// <summary>
    /// Gets the tile position.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>The tile position.</returns>
    public Vector2 GetTilePosition(Point tile);
}

/// <summary>
/// An entity which contains a grid.
/// </summary>
public class GridContainer : Entity, IGridContainer {
    private Vector2 _tileSize = Vector2.One;

    /// <summary>
    /// Creates a new instance of <see cref="GridContainer" />.
    /// </summary>
    public GridContainer() : base() {
    }

    /// <summary>
    /// An empty grid container. Defaults to a 1 by 1 grid.
    /// </summary>
    public new static IGridContainer Empty => EmptyObject.Instance;

    /// <inheritdoc />
    [DataMember(Name = "Tile Size")]
    [Category(CommonCategories.Grid)]
    public Vector2 TileSize {
        get => this._tileSize;
        set => this.Set(ref this._tileSize, value);
    }

    /// <inheritdoc />
    public Vector2 GetNearestTilePosition(Vector2 position) {
        var worldPosition = this.WorldPosition;
        var x = position.X - worldPosition.X;
        var y = position.Y - worldPosition.Y;

        if (this.TileSize.X > 0f) {
            x = (float)Math.Round(x / this.TileSize.X) * this.TileSize.X;
        }

        if (this.TileSize.Y > 0f) {
            y = (float)Math.Round(y / this.TileSize.Y) * this.TileSize.Y;
        }

        return new Vector2(x, y) + worldPosition;
    }


    /// <summary>
    /// Gets the tile position.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>The tile position.</returns>
    public Vector2 GetTilePosition(Point tile) {
        return new Vector2(tile.X * this.TileSize.X, tile.Y * this.TileSize.Y) + this.WorldPosition;
    }
}