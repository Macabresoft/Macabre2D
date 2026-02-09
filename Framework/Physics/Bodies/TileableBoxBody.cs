namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A tileable body that creates rectangle colliders to fill out the active tiles on a <see cref="ITileableEntity" />.
/// </summary>
public class TileableBoxBody : PhysicsBody {
    private readonly List<Collider> _colliders = [];
    private Orientation _colliderOrientation;
    private ITileableEntity? _tileable;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._tileable?.BoundingArea ?? BoundingArea.Empty;

    /// <summary>
    /// Gets or sets the orientation of this collider.
    /// </summary>
    [DataMember]
    public Orientation ColliderOrientation {
        get => this._colliderOrientation;
        set {
            if (value != this._colliderOrientation) {
                this._colliderOrientation = value;

                if (this.IsInitialized) {
                    this.ResetColliders();
                }
            }
        }
    }

    /// <inheritdoc />
    public override bool HasCollider => this._colliders.Any();

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        foreach (var collider in this._colliders) {
            collider.Deinitialize();
        }

        if (this._tileable != null) {
            this._tileable.TilesChanged -= this.OnRequestReset;
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() => this._colliders;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this.TryGetAncestor(out this._tileable) && this._tileable != null) {
            this._tileable.TilesChanged += this.OnRequestReset;
            this.ResetColliders();
        }
        else {
            this._tileable = null;
        }
    }

    private void OnRequestReset(object? sender, EventArgs e) {
        this.ResetColliders();
    }

    private void ResetColliders() {
        this._colliders.Clear();

        if (this._tileable is { CurrentGrid: not EmptyObject } && this._tileable.ActiveTiles.Any()) {
            var chunks = this._colliderOrientation == Orientation.Horizontal ? TileChunk.GetRowChunks(this._tileable.ActiveTiles) : TileChunk.GetColumnChunks(this._tileable.ActiveTiles);

            foreach (var chunk in chunks) {
                var minimum = this._tileable.CurrentGrid.GetTilePosition(chunk.MinimumTile);
                var maximum = this._tileable.CurrentGrid.GetTilePosition(new Point(chunk.MaximumTile.X + 1, chunk.MaximumTile.Y + 1));

                var collider = new RectangleCollider(minimum, maximum) {
                    Layers = this.Layers
                };

                collider.Initialize(this);
                this._colliders.Add(collider);
            }
        }
    }
}