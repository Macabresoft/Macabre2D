namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
[Display(Name = "Simple Physics Body")]
public class SimplePhysicsBody : PhysicsBody, ISimplePhysicsBody {
    private Collider _collider = new CircleCollider();

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this.Collider.BoundingArea;

    /// <inheritdoc />
    public override bool HasCollider => true;

    /// <inheritdoc />
    [DataMember(Order = 0)]
    [Category("Collider")]
    public Collider Collider {
        get => this._collider;
        set {
            this._collider.BoundingAreaChanged -= this.Collider_BoundingAreaChanged;

            if (this.Set(ref this._collider, value) && this.IsInitialized) {
                this.InitializeCollider();
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() {
        return new[] { this.Collider };
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.InitializeCollider();
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this._collider.Reset();
    }

    private void Collider_BoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void InitializeCollider() {
        this._collider.Initialize(this);
        this._collider.BoundingAreaChanged += this.Collider_BoundingAreaChanged;
    }
}