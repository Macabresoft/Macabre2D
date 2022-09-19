namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

/// <summary>
/// A readonly interface for a <see cref="QuadTree{T}"/>.
/// </summary>
/// <typeparam name="T">The type contained in this quad tree.</typeparam>
public interface IReadonlyQuadTree<T> where T : IBoundable {
    /// <summary>
    /// Retrieves all colliders which could potentially be colliding with the specified collider.
    /// </summary>
    /// <param name="boundable">The boundable.</param>
    /// <returns>All potential collision colliders.</returns>
    IEnumerable<T> RetrievePotentialCollisions(T boundable);

    /// <summary>
    /// Retrieves all colliders which could potentially be in the specified bounding area.
    /// </summary>
    /// <param name="boundingArea">The bounding area.</param>
    /// <returns>All potential collision colliders.</returns>
    IEnumerable<T> RetrievePotentialCollisions(BoundingArea boundingArea);
}

/// <summary>
/// A quad tree used in detecting collisions. I read the tutorial found at the following link to
/// construct this class:
/// https://gamedevelopment.tutsplus.com/tutorials/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space--gamedev-374
/// </summary>
public sealed class QuadTree<T> : IReadonlyQuadTree<T> where T : IBoundable {
    private readonly Vector2 _bottomLeftBounds;
    private readonly List<T> _boundables = new();
    private readonly int _depth;
    private readonly QuadTree<T>[] _nodes = new QuadTree<T>[4];
    private readonly Vector2 _topRightBounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuadTree{T}" /> class.
    /// </summary>
    /// <param name="depth">The depth.</param>
    /// <param name="x">The left most position of the tree's bounds.</param>
    /// <param name="y">The bottom most position of the tree's bounds.</param>
    /// <param name="width">The width of the tree's bounds.</param>
    /// <param name="height">The height of the tree's bounds.</param>
    public QuadTree(int depth, float x, float y, float width, float height) {
        this._depth = depth;
        this._bottomLeftBounds = new Vector2(x, y);
        this._topRightBounds = this._bottomLeftBounds + new Vector2(width, height);
    }

    /// <summary>
    /// Gets or sets the maximum levels.
    /// </summary>
    /// <value>The maximum levels.</value>
    public int MaxLevels { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum objects.
    /// </summary>
    /// <value>The maximum objects.</value>
    public int MaxObjects { get; set; } = 10;

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear() {
        this._boundables.Clear();

        // NOTE: I'm breaking this contract and I don't care!!!
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (this._nodes[0] != null) {
            foreach (var node in this._nodes) {
                node.Clear();
            }
        }
    }

    /// <summary>
    /// Inserts the item into its proper position in the quad tree.
    /// </summary>
    /// <param name="item">The item to insert.</param>
    public void Insert(T item) {
        // We only have to null check the first node, because we always add every node at once.
        // NOTE: I'm breaking this contract and I don't care!!!
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var hasNodes = this._nodes[0] != null;

        if (hasNodes) {
            var boundingArea = item.BoundingArea;
            var quadrant = this.GetQuadrant(boundingArea);

            if (quadrant != Quadrant.None) {
                this._nodes[(int)quadrant].Insert(item);
                return;
            }
        }

        this._boundables.Add(item);

        if (this._boundables.Count >= this.MaxObjects && this._depth < this.MaxLevels) {
            if (!hasNodes) {
                this.Split();
            }

            for (var i = this._boundables.Count - 1; i >= 0; i--) {
                var collider = this._boundables[i];
                var boundingArea = collider.BoundingArea;
                var quadrant = this.GetQuadrant(boundingArea);

                if (quadrant != Quadrant.None) {
                    this._nodes[(int)quadrant].Insert(collider);
                    this._boundables.Remove(collider);
                }
            }
        }
    }

    /// <summary>
    /// Inserts all provided items into their proper position in the quad tree.
    /// </summary>
    /// <param name="items">The items.</param>
    public void InsertMany(IEnumerable<T> items) {
        foreach (var item in items) {
            this.Insert(item);
        }
    }

    /// <inheritdoc />
    public IEnumerable<T> RetrievePotentialCollisions(T boundable) {
        return this.RetrievePotentialCollisions(boundable.BoundingArea);
    }

    /// <inheritdoc />
    public IEnumerable<T> RetrievePotentialCollisions(BoundingArea boundingArea) {
        var quadrant = this.GetQuadrant(boundingArea);
        IEnumerable<T> result = this._boundables;
        if (quadrant != Quadrant.None) {
            // We can directly add to boundables here, because boundables is cleared every frame
            // This is much better than allocating a new collection every frame that contains every
            // boundable in the game.
            result = result.Concat(this._nodes[(int)quadrant].RetrievePotentialCollisions(boundingArea));
        }

        return result;
    }

    private Quadrant GetQuadrant(BoundingArea boundingArea) {
        var x = boundingArea.Minimum.X;
        var y = boundingArea.Minimum.Y;
        var width = boundingArea.Maximum.X - x;
        var height = boundingArea.Maximum.Y - y;

        var quadrant = Quadrant.None;
        var verticalMidpoint = x + height * 0.5f;
        var horizontalMidPoint = x + width * 0.5f;

        var isBottomQuadrant = y < verticalMidpoint && y + height < verticalMidpoint;
        var isTopQuadrant = y > verticalMidpoint;

        if (x < horizontalMidPoint && x + width < horizontalMidPoint) {
            if (isBottomQuadrant) {
                quadrant = Quadrant.BottomLeft;
            }
            else if (isTopQuadrant) {
                quadrant = Quadrant.TopLeft;
            }
        }
        else if (x > horizontalMidPoint) {
            if (isBottomQuadrant) {
                quadrant = Quadrant.BottomRight;
            }
            else if (isTopQuadrant) {
                quadrant = Quadrant.TopRight;
            }
        }

        return quadrant;
    }

    private void Split() {
        var newWidth = (this._topRightBounds.X - this._bottomLeftBounds.X) * 0.5f;
        var newHeight = (this._topRightBounds.Y - this._bottomLeftBounds.Y) * 0.5f;

        this._nodes[(int)Quadrant.BottomRight] = new QuadTree<T>(
            this._depth + 1,
            this._bottomLeftBounds.X + newWidth,
            this._bottomLeftBounds.Y,
            newWidth,
            newHeight);

        this._nodes[(int)Quadrant.BottomLeft] = new QuadTree<T>(
            this._depth + 1,
            this._bottomLeftBounds.X,
            this._bottomLeftBounds.Y,
            newWidth,
            newHeight);

        this._nodes[(int)Quadrant.TopLeft] = new QuadTree<T>(
            this._depth + 1,
            this._bottomLeftBounds.X,
            this._bottomLeftBounds.Y + newHeight,
            newWidth,
            newHeight);

        this._nodes[(int)Quadrant.TopRight] = new QuadTree<T>(
            this._depth + 1,
            this._bottomLeftBounds.X + newWidth,
            this._bottomLeftBounds.Y + newHeight,
            newWidth,
            newHeight);
    }

    /// <summary>
    /// Potential quadrants for children.
    /// </summary>
    private enum Quadrant {
        None = -1,
        BottomLeft = 0,
        BottomRight = 1,
        TopLeft = 2,
        TopRight = 3
    }
}