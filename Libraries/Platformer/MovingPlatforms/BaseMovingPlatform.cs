namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A base class for <see cref="IMovingPlatform" />.
/// </summary>
public class BaseMovingPlatform : Entity, IMovingPlatform {
    private readonly HashSet<IPlatformerActor> _attached = new();
    private readonly LineCollider _collider = new();
    private Layers _colliderLayers;
    private float _platformLength;
    private Vector2 _platformOffset;
    private Vector2 _previousPosition;
    private int _updateOrder;

    public event EventHandler<CollisionEventArgs>? CollisionOccured;

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
    public void Attach(IPlatformerActor actor) {
        this._attached.Add(actor);
    }

    /// <inheritdoc />
    public void Detach(IPlatformerActor actor) {
        this._attached.Remove(actor);
    }

    /// <inheritdoc />
    public IEnumerable<Collider> GetColliders() {
        return new[] { this.Collider };
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._previousPosition = this.Transform.Position;
        this._collider.Initialize(this);
        this.ResetCollider();
    }

    /// <inheritdoc />
    public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
        this.CollisionOccured.SafeInvoke(this, eventArgs);
    }

    /// <summary>
    /// Moves the attached actors.
    /// </summary>
    /// <param name="amount">The amount to move attached actors.</param>
    protected void MoveAttached(Vector2 amount) {
        if (this._attached.Any()) {
            var polygonCollider = this.Collider as PolygonCollider;
            var adjustForY = amount.Y != 0f && polygonCollider != null && polygonCollider.WorldPoints.Any();
            var adjustForPixels = this.Settings.SnapToPixels && this.Transform.Position != this._previousPosition && amount.X != 0f;
            var settings = this.Settings;
            var platformPixelOffset = this.Transform.ToPixelSnappedValue(settings).Position.X - this.Transform.Position.X;

            foreach (var attached in this._attached) {
                attached.Move(amount);
                if (adjustForPixels && attached.CurrentState.Velocity.X == 0f) {
                    attached.SetWorldPosition(new Vector2(attached.Transform.ToPixelSnappedValue(settings).Position.X - platformPixelOffset, attached.Transform.Position.Y));
                }

                if (adjustForY) {
                    var yValue = polygonCollider?.WorldPoints.Select(x => x.Y).Max() ?? attached.Transform.Position.Y;
                    attached.SetWorldPosition(new Vector2(attached.Transform.Position.X, yValue + attached.HalfSize.Y));
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.Transform)) {
            this.ResetCollider();
            this.MoveAttached(this.Transform.Position - this._previousPosition);
            this._previousPosition = this.Transform.Position;
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