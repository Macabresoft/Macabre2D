namespace Macabresoft.Macabre2D.Libraries.Platformer.Walls;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A wall to which actors can attach.
/// </summary>
public class Wall : Attachable, ISimplePhysicsBody {
    private readonly LineCollider _collider = new();
    private Layers _colliderLayers;
    private int _updateOrder;
    private float _wallHeight;
    private Vector2 _wallOffset;

    /// <inheritdoc />
    public event EventHandler<CollisionEventArgs>? CollisionOccured;

    /// <inheritdoc />
    public BoundingArea BoundingArea => this.Collider.BoundingArea;

    /// <inheritdoc />
    public Collider Collider => this._collider;

    /// <inheritdoc />
    public bool HasCollider => this.WallHeight > 0f;

    /// <inheritdoc />
    public bool IsTrigger => false;

    /// <summary>
    /// Gets or sets the layers for the collider.
    /// </summary>
    [DataMember]
    public Layers ColliderLayers {
        get => this._colliderLayers;
        set {
            if (this.Set(ref this._colliderLayers, value)) {
                this.ResetCollider();
            }
        }
    }

    /// <inheritdoc />
    [DataMember(Order = 2, Name = "Physics Material")]
    public PhysicsMaterial PhysicsMaterial { get; set; } = PhysicsMaterial.Default;

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get => this._updateOrder;
        set => this.Set(ref this._updateOrder, value);
    }

    /// <summary>
    /// Gets or sets the wall collider height.
    /// </summary>
    [DataMember]
    public float WallHeight {
        get => this._wallHeight;
        set {
            if (this.Set(ref this._wallHeight, value)) {
                this.ResetCollider();
            }
        }
    }

    /// <summary>
    /// Gets or sets the wall collider offset.
    /// </summary>
    [DataMember]
    public Vector2 WallOffset {
        get => this._wallOffset;
        set {
            if (this.Set(ref this._wallOffset, value)) {
                this.ResetCollider();
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<Collider> GetColliders() {
        return new[] { this.Collider };
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this._collider.Initialize(this);
        this.ResetCollider();
    }

    /// <inheritdoc />
    public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
        this.CollisionOccured.SafeInvoke(this, eventArgs);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.Transform)) {
            this.ResetCollider();
        }
    }

    private void ResetCollider() {
        if (this.IsInitialized) {
            this._collider.Start = this.WallOffset;
            this._collider.End = new Vector2(this.WallOffset.X, this.WallOffset.Y + this._wallHeight);
            this._collider.Layers = this._colliderLayers;
        }
    }
}