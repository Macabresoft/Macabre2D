namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

/// <summary>
/// A chunk of tiles for handling colliders and appearances of a <see cref="ITileableEntity" />.
/// </summary>
public class TileChunk {
    /// <summary>
    /// Initializes a new instance of <see cref="TileChunk" />.
    /// </summary>
    /// <param name="tile">The tile.</param>
    public TileChunk(Point tile) {
        this.MinimumTile = tile;
        this.MaximumTile = tile;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TileChunk" />.
    /// </summary>
    /// <param name="minimumTile">The minimum tile.</param>
    /// <param name="maximumTile">The maximum tile.</param>
    public TileChunk(Point minimumTile, Point maximumTile) {
        this.MinimumTile = minimumTile;
        this.MaximumTile = maximumTile;
    }

    /// <summary>
    /// Gets or sets the maximum tile.
    /// </summary>
    public Point MaximumTile { get; set; }

    /// <summary>
    /// Gets or sets the minimum tile.
    /// </summary>
    public Point MinimumTile { get; set; }

    /// <summary>
    /// Gets the chunks as a set of columns given the active tiles.
    /// </summary>
    /// <param name="activeTiles">The active tiles.</param>
    /// <returns>The chunks built by the active tiles.</returns>
    public static IEnumerable<TileChunk> GetColumnChunks(IReadOnlyCollection<Point> activeTiles) {
        IEnumerable<TileChunk> chunks;
        if (activeTiles.Any()) {
            var tempChunks = GetColumns(activeTiles);
            var reversedVisualChunks = tempChunks.OrderByDescending(x => x.MaximumTile.X).ToList();
            foreach (var visualChunk in reversedVisualChunks) {
                var candidates = tempChunks.Where(x => x.MaximumTile.X == visualChunk.MinimumTile.X - 1).ToList();

                if (candidates.Any(candidate => candidate.TryCombineColumn(visualChunk))) {
                    tempChunks.Remove(visualChunk);
                }
            }

            chunks = tempChunks;
        }
        else {
            chunks = [];
        }

        return chunks;
    }


    /// <summary>
    /// Gets the chunks as a set of uncombined rows given the active tiles.
    /// </summary>
    /// <param name="activeTiles">The active tiles.</param>
    /// <returns>The chunks built by the active tiles.</returns>
    public static IEnumerable<TileChunk> GetIndividualColumnsAsChunks(IReadOnlyCollection<Point> activeTiles) => GetColumns(activeTiles);

    /// <summary>
    /// Gets the chunks as a set of uncombined rows given the active tiles.
    /// </summary>
    /// <param name="activeTiles">The active tiles.</param>
    /// <returns>The chunks built by the active tiles.</returns>
    public static IEnumerable<TileChunk> GetIndividualRowsAsChunks(IReadOnlyCollection<Point> activeTiles) => GetRows(activeTiles);

    /// <summary>
    /// Gets the chunks as a set of rows given the active tiles.
    /// </summary>
    /// <param name="activeTiles">The active tiles.</param>
    /// <returns>The chunks built by the active tiles.</returns>
    public static IEnumerable<TileChunk> GetRowChunks(IReadOnlyCollection<Point> activeTiles) {
        IEnumerable<TileChunk> chunks;
        if (activeTiles.Any()) {
            var tempChunks = GetRows(activeTiles);

            var reversedVisualChunks = tempChunks.OrderByDescending(x => x.MaximumTile.Y).ToList();
            foreach (var visualChunk in reversedVisualChunks) {
                var candidates = tempChunks.Where(x => x.MaximumTile.Y == visualChunk.MinimumTile.Y - 1).ToList();

                if (candidates.Any(candidate => candidate.TryCombineRow(visualChunk))) {
                    tempChunks.Remove(visualChunk);
                }
            }

            chunks = tempChunks;
        }
        else {
            chunks = [];
        }

        return chunks;
    }

    /// <summary>
    /// Tries to add a tile to a column.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>A value indicating whether or not the tile was added.</returns>
    public bool TryAddToColumn(Point tile) {
        if (this.MaximumTile.X == this.MinimumTile.X) {
            if (tile.Y == this.MinimumTile.Y - 1) {
                this.MinimumTile = tile;
                return true;
            }

            if (tile.Y == this.MaximumTile.Y + 1) {
                this.MaximumTile = tile;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to add a tile to a row.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>A value indicating whether or not the tile was added.</returns>
    public bool TryAddToRow(Point tile) {
        if (this.MaximumTile.Y == this.MinimumTile.Y) {
            if (tile.X == this.MinimumTile.X - 1) {
                this.MinimumTile = tile;
                return true;
            }

            if (tile.X == this.MaximumTile.X + 1) {
                this.MaximumTile = tile;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to combine two column chunks.
    /// </summary>
    /// <param name="other">The other chunk.</param>
    /// <returns>A value indicating whether or not the chunks were combined.</returns>
    public bool TryCombineColumn(TileChunk other) {
        if (this.MinimumTile.Y == other.MinimumTile.Y && this.MaximumTile.Y == other.MaximumTile.Y) {
            if (this.MaximumTile.X == other.MinimumTile.X - 1 || this.MinimumTile.X + 1 == other.MaximumTile.X) {
                this.MaximumTile = new Point(Math.Max(this.MaximumTile.X, other.MaximumTile.X), this.MaximumTile.Y);
                this.MinimumTile = new Point(Math.Min(this.MinimumTile.X, other.MinimumTile.X), this.MinimumTile.Y);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to combine two row chunks.
    /// </summary>
    /// <param name="other">The other chunk.</param>
    /// <returns>A value indicating whether or not the chunks were combined.</returns>
    public bool TryCombineRow(TileChunk other) {
        if (this.MinimumTile.X == other.MinimumTile.X && this.MaximumTile.X == other.MaximumTile.X) {
            if (this.MaximumTile.Y == other.MinimumTile.Y - 1 || this.MinimumTile.Y + 1 == other.MaximumTile.Y) {
                this.MaximumTile = new Point(this.MaximumTile.X, Math.Max(this.MaximumTile.Y, other.MaximumTile.Y));
                this.MinimumTile = new Point(this.MinimumTile.X, Math.Min(this.MinimumTile.Y, other.MinimumTile.Y));
                return true;
            }
        }

        return false;
    }

    private static List<TileChunk> GetColumns(IReadOnlyCollection<Point> activeTiles) {
        var chunks = new List<TileChunk>();
        if (activeTiles.Any()) {
            var orderedTiles = activeTiles.OrderBy(tile => tile.X).ThenBy(tile => tile.Y).ToList();
            var currentVisualChunk = new TileChunk(orderedTiles.First());
            chunks.Add(currentVisualChunk);
            orderedTiles.RemoveAt(0);
            
            foreach (var activeTile in orderedTiles.Where(activeTile => !currentVisualChunk.TryAddToColumn(activeTile))) {
                currentVisualChunk = new TileChunk(activeTile);
                chunks.Add(currentVisualChunk);
            }
        }

        return chunks;
    }

    private static List<TileChunk> GetRows(IReadOnlyCollection<Point> activeTiles) {
        var chunks = new List<TileChunk>();
        if (activeTiles.Any()) {
            var orderedTiles = activeTiles.OrderBy(tile => tile.Y).ThenBy(tile => tile.X).ToList();
            var currentVisualChunk = new TileChunk(orderedTiles.First());
            chunks.Add(currentVisualChunk);
            orderedTiles.RemoveAt(0);

            foreach (var activeTile in orderedTiles.Where(activeTile => !currentVisualChunk.TryAddToRow(activeTile))) {
                currentVisualChunk = new TileChunk(activeTile);
                chunks.Add(currentVisualChunk);
            }
        }

        return chunks;
    }
}