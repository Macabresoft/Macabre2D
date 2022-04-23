namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A base interface for blocks.
/// </summary>
public interface IBlock {
    /// <summary>
    /// Gets the layers for the bottom collider of this block.
    /// </summary>
    Layers BottomLayers { get; }

    /// <summary>
    /// Gets the layers for the colliders on the sides of this block.
    /// </summary>
    Layers SideLayers { get; }

    /// <summary>
    /// Gets the size of this block.
    /// </summary>
    Vector2 Size { get; }

    /// <summary>
    /// Gets the layers for the top collider of this block.
    /// </summary>
    Layers TopLayers { get; }

    /// <summary>
    /// Called when an actor hits this block.
    /// </summary>
    void Hit();
}

/// <summary>
/// Base class for
/// </summary>
public abstract class Block : PhysicsBody, IBlock {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly BlockColliderCollection _colliders = new();
    private Layers _bottomLayers;
    private Layers _sideLayers;
    private Vector2 _size = Vector2.One;
    private Layers _topLayers;

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity" /> class.
    /// </summary>
    protected Block() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <inheritdoc />
    public override bool HasCollider => this._colliders.Any();

    /// <inheritdoc />
    [DataMember(Order = 102)]
    public Layers BottomLayers {
        get => this._bottomLayers;
        set {
            if (this.Set(ref this._bottomLayers, value)) {
                this._colliders.BottomCollider.Layers = this._bottomLayers;
            }
        }
    }

    /// <inheritdoc />
    [DataMember(Order = 100)]
    public Layers SideLayers {
        get => this._sideLayers;
        set {
            if (this.Set(ref this._sideLayers, value)) {
                this._colliders.LeftCollider.Layers = this._sideLayers;
                this._colliders.RightCollider.Layers = this._sideLayers;
            }
        }
    }

    /// <inheritdoc />
    public Vector2 Size {
        get => this._size;
        set {
            if (this.Set(ref this._size, value)) {
                this._boundingArea.Reset();
                this.ResetColliders();
            }
        }
    }

    /// <inheritdoc />
    [DataMember(Order = 101)]
    public Layers TopLayers {
        get => this._topLayers;
        set {
            if (this.Set(ref this._topLayers, value)) {
                this._colliders.TopCollider.Layers = this._topLayers;
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() {
        return this._colliders;
    }

    /// <inheritdoc />
    public abstract void Hit();

    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        foreach (var collider in this._colliders) {
            collider.Initialize(this);
        }

        this.ResetColliders();
    }


    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(ITransformable.Transform)) {
            this._boundingArea.Reset();
            this.ResetColliders();
        }
    }

    private BoundingArea CreateBoundingArea() {
        var size = this.Size * this.Transform.Scale;
        return new BoundingArea(this.Transform.Position, size.X, size.Y);
    }

    private void ResetColliders() {
        this._colliders.LeftCollider.Start = Vector2.Zero;
        this._colliders.LeftCollider.End = new Vector2(0f, this.Size.Y);
        this._colliders.LeftCollider.Layers = this.SideLayers;

        this._colliders.RightCollider.Start = new Vector2(this.Size.X, 0f);
        this._colliders.RightCollider.End = new Vector2(this.Size.X, this.Size.Y);
        this._colliders.RightCollider.Layers = this.SideLayers;

        this._colliders.TopCollider.Start = new Vector2(0f, this.Size.Y);
        this._colliders.TopCollider.End = new Vector2(this.Size.X, this.Size.Y);
        this._colliders.TopCollider.Layers = this.TopLayers;

        this._colliders.BottomCollider.Start = Vector2.Zero;
        this._colliders.BottomCollider.End = new Vector2(this.Size.X, 0f);
        this._colliders.BottomCollider.Layers = this.BottomLayers;
    }
}