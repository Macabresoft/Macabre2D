namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A tileable body that creates rectangle colliders to fill out the active tiles on a <see cref="ITileableEntity" />.
/// </summary>
[Display(Name = "Tileable Body (Box)")]
public class TileableBoxBody : PhysicsBody {
    private readonly List<Collider> _colliders = new();
    private Orientation _colliderOrientation;
    private ITileableEntity? _tileable;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._tileable?.BoundingArea ?? new BoundingArea();

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override bool HasCollider => this._colliders.Any();

    [DataMember]
    public Orientation ColliderOrientation {
        get => this._colliderOrientation;
        set {
            if (value != this._colliderOrientation) {
                this._colliderOrientation = value;
                this.ResetColliders();
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() {
        return this._colliders;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this._tileable != null) {
            this._tileable.TilesChanged -= this.OnRequestReset;
        }

        if (this.TryGetParentEntity(out this._tileable) && this._tileable != null) {
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