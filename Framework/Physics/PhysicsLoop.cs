namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A physics system that handles collisions.
/// </summary>
public interface IPhysicsLoop : ISimplePhysicsLoop {
    /// <summary>
    /// Gets or sets the gravity.
    /// </summary>
    /// <value>The gravity.</value>
    Gravity Gravity { get; }

    /// <summary>
    /// Gets the groundedness. This is a value indicating how likely a body is to be grounded.
    /// It should be a value between 0 and 1, where 0.5 would (theoretically) allow an object to
    /// be grounded at a 45 degree angle.
    /// </summary>
    /// <value>The groundedness.</value>
    float Groundedness { get; }

    /// <summary>
    /// Gets the minimum magnitude of velocity perpendicular to the collision after bounce has
    /// been applied.
    /// </summary>
    /// <value>The minimum post bounce magnitude.</value>
    float MinimumPostBounceMagnitude { get; }

    /// <summary>
    /// Gets the minimum magnitude of velocity parallel to the collision after friction has been applied.
    /// </summary>
    /// <value>The minimum post friction magnitude.</value>
    float MinimumPostFrictionMagnitude { get; }

    /// <summary>
    /// Gets the stickiness. This is a value indicating how likely a body is to "stick" to
    /// another object when it is moving in a similar path to the other body's edge. This should
    /// be a value between 0 and 1, with 0 not being sticky at all and 1 sticking all of the time.
    /// </summary>
    /// <value>The stickiness.</value>
    float Stickiness { get; }
}

/// <summary>
/// A system which allows for raycasting and handles rigidbody physics interactions.
/// </summary>
/// <seealso cref="SimplePhysicsLoop" />
[Category(CommonCategories.Physics)]
public class PhysicsLoop : SimplePhysicsLoop, IPhysicsLoop {
    private readonly Dictionary<Guid, List<Guid>> _collisionsHandled = new();

    [DataMember(Name = "Collision Resolver", Order = 0)]
    private ICollisionResolver _collisionResolver = new DefaultCollisionResolver();

    private float _groundedness = 0.35f;
    private float _minimumPostBounceMagnitude = 1.5f;
    private float _minimumPostFrictionMagnitude = 0.2f;
    private float _stickiness = 0.1f;

    /// <summary>
    /// Gets the collision map.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.CollisionMap)]
    public CollisionMap CollisionMap { get; } = new();

    /// <inheritdoc />
    [DataMember(Order = 1)]
    public Gravity Gravity { get; } = new(Vector2.Zero);

    /// <summary>
    /// Gets the collision resolver.
    /// </summary>
    /// <value>The collision resolver.</value>
    public ICollisionResolver CollisionResolver {
        get => this._collisionResolver;
        private set => this.Set(ref this._collisionResolver, value);
    }

    /// <inheritdoc />
    [DataMember(Order = 2)]
    public float Groundedness {
        get => this._groundedness;
        set => this.Set(ref this._groundedness, value);
    }

    /// <inheritdoc />
    [DataMember(Order = 4, Name = "Minimum Post-Bounce Magnitude")]
    public float MinimumPostBounceMagnitude {
        get => this._minimumPostBounceMagnitude;
        set => this.Set(ref this._minimumPostBounceMagnitude, value);
    }

    /// <inheritdoc />
    [DataMember(Order = 5, Name = "Minimum Post-Friction Magnitude")]
    public float MinimumPostFrictionMagnitude {
        get => this._minimumPostFrictionMagnitude;
        set => this.Set(ref this._minimumPostFrictionMagnitude, value);
    }

    /// <inheritdoc />
    [DataMember(Order = 3)]
    public float Stickiness {
        get => this._stickiness;
        set => this.Set(ref this._stickiness, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);
        this._collisionResolver.Initialize(this);
    }

    /// <inheritdoc />
    protected override void FixedUpdate(FrameTime frameTime, InputState inputState) {
        base.FixedUpdate(frameTime, inputState);
        this._collisionsHandled.Clear();

        foreach (var body in this.Scene.PhysicsBodies) {
            this.HandleCollisions(body);
        }
    }

    private void HandleCollisions(IPhysicsBody body) {
        if (body.HasCollider && body is IDynamicPhysicsBody dynamicBody) {
            var collisionsOccured = new List<Guid>();
            dynamicBody.SetWorldPosition(dynamicBody.Transform.Position + dynamicBody.Velocity * this.TimeStep);

            if (dynamicBody.IsKinematic) {
                dynamicBody.Velocity += this.Gravity.Value * this.TimeStep;
            }

            foreach (var collider in body.GetColliders()) {
                var potentials = this.ColliderTree.RetrievePotentialCollisions(collider);
                foreach (var otherCollider in potentials.Where(c => c != collider && this.CollisionMap.GetShouldCollide(c.Layers, collider.Layers))) {
                    if (otherCollider.Body != null) {
                        var hasCollisionAlreadyResolved = this._collisionsHandled.TryGetValue(otherCollider.Body.Id, out var collisions) &&
                                                          collisions.Contains(body.Id);

                        if (!hasCollisionAlreadyResolved && collider.CollidesWith(otherCollider, out var collision) && collision != null) {
                            if (!body.IsTrigger && !otherCollider.Body.IsTrigger) {
                                this._collisionResolver.ResolveCollision(collision, this.TimeStep);
                            }

                            body.NotifyCollisionOccured(collision);

                            var otherCollision = new CollisionEventArgs(
                                collision.SecondCollider,
                                collision.FirstCollider,
                                collision.Normal,
                                -collision.MinimumTranslationVector,
                                collision.SecondContainsFirst,
                                collision.FirstContainsSecond);
                            otherCollider.Body.NotifyCollisionOccured(otherCollision);
                            collisionsOccured.Add(otherCollider.Body.Id);
                        }
                    }
                }

                this._collisionsHandled[body.Id] = collisionsOccured;
            }
        }
    }
}