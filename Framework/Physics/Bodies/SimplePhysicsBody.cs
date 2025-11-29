namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A <see cref="IPhysicsBody" /> with a <see cref="Collider" />.
/// </summary>
public interface ISimplePhysicsBody : IPhysicsBody {
    /// <summary>
    /// Gets the colliders
    /// </summary>
    Collider Collider { get; }
}

/// <summary>
/// A body to be used by the physics engine.
/// </summary>
public class SimplePhysicsBody : PhysicsBody, ISimplePhysicsBody {
    private Collider _collider = new CircleCollider();

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this.Collider.BoundingArea;

    /// <inheritdoc />
    [DataMember(Order = 0)]
    [Category("Collider")]
    public Collider Collider {
        get => this._collider;
        set {
            this._collider.BoundingAreaChanged -= this.Collider_BoundingAreaChanged;
            this._collider = value;
            this.OnColliderChanged();
        }
    }

    /// <inheritdoc />
    public override bool HasCollider => true;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._collider.Deinitialize();
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() => [this.Collider];

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.InitializeCollider();
    }

    /// <summary>
    /// Called when the collider changes.
    /// </summary>
    protected virtual void OnColliderChanged() {
        if (this.IsInitialized) {
            this.InitializeCollider();
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void Collider_BoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void InitializeCollider() {
        this._collider.Initialize(this);
        this._collider.BoundingAreaChanged += this.Collider_BoundingAreaChanged;
    }
}