namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A base class for all platforms.
/// </summary>
public class Platform : Entity, ISimplePhysicsBody {
    private readonly LineCollider _collider = new();
    private Layers _colliderLayers;
    private float _platformLength;
    private Vector2 _platformOffset;
    private int _updateOrder;

    /// <inheritdoc />
    public event EventHandler<CollisionEventArgs>? CollisionOccured;

    /// <inheritdoc />
    public BoundingArea BoundingArea => this.Collider.BoundingArea;

    /// <inheritdoc />
    public Collider Collider => this._collider;

    /// <inheritdoc />
    public bool HasCollider => this.PlatformLength > 0f;

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

    /// <summary>
    /// Gets or sets the platform collider length.
    /// </summary>
    [DataMember]
    public float PlatformLength {
        get => this._platformLength;
        set {
            if (this.Set(ref this._platformLength, value)) {
                this.ResetCollider();
            }
        }
    }

    /// <summary>
    /// Gets or sets the platform collider offset.
    /// </summary>
    [DataMember]
    public Vector2 PlatformOffset {
        get => this._platformOffset;
        set {
            if (this.Set(ref this._platformOffset, value)) {
                this.ResetCollider();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get => this._updateOrder;
        set => this.Set(ref this._updateOrder, value);
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
            this._collider.Start = this.PlatformOffset;
            this._collider.End = new Vector2(this.PlatformOffset.X + this._platformLength, this.PlatformOffset.Y);
            this._collider.Layers = this._colliderLayers;
        }
    }
}