namespace Macabresoft.Macabre2D.Framework;

using System;
using Microsoft.Xna.Framework;

/// <summary>
/// The default collision resolver. This will be used
/// </summary>
/// <seealso cref="ICollisionResolver" />
public sealed class DefaultCollisionResolver : ICollisionResolver {
    private IPhysicsSystem? _system;

    /// <inheritdoc />
    public void Initialize(IPhysicsSystem system) {
        this._system = system;
    }

    /// <inheritdoc />
    public void ResolveCollision(CollisionEventArgs e, float timeStep) {
        if (e.FirstCollider.Body is IDynamicPhysicsBody firstBody) {
            firstBody.SetWorldPosition(firstBody.Transform.Position + e.MinimumTranslationVector);

            if (firstBody.IsKinematic) {
                var normalPerpendicular = e.Normal.GetPerpendicular();

                if (e.SecondCollider.Body is IDynamicPhysicsBody { IsKinematic: true } otherBody) {
                    var firstBodyMagnitude = firstBody.Velocity.Length();
                    var otherBodyMagnitude = otherBody.Velocity.Length();

                    var firstMassMultiplier = firstBody.Mass * firstBodyMagnitude;
                    var secondMassMultiplier = otherBody.Mass * otherBodyMagnitude;

                    var totalMass = firstMassMultiplier + secondMassMultiplier;
                    var massRatio = totalMass == 0f ? 0.5f : firstMassMultiplier / totalMass;
                    var inverseMassRatio = 1f - massRatio;

                    var bounce = (firstBody.PhysicsMaterial.Bounce + otherBody.PhysicsMaterial.Bounce) * 0.5f;
                    var firstReflection = this.GetReflectedVelocity(firstBody, e.Normal, bounce);
                    var otherReflection = this.GetReflectedVelocity(otherBody, e.Normal, bounce);

                    var averageMagnitude = (firstBodyMagnitude + otherBodyMagnitude) * 0.5f;
                    firstBody.Velocity = massRatio * firstReflection + averageMagnitude * inverseMassRatio * e.Normal;
                    otherBody.Velocity = inverseMassRatio * otherReflection + averageMagnitude * massRatio * -e.Normal;
                }
                else if (e.SecondCollider.Body is IPhysicsBody secondBody) {
                    firstBody.Velocity = this.GetNewVelocity(firstBody, secondBody, e.Normal, normalPerpendicular, timeStep);
                }
            }
        }
    }

    private Vector2 ApplyFriction(Vector2 velocity, Vector2 normalPerpendicular, float friction) {
        if (friction == 0 || this._system == null) {
            return velocity;
        }

        var frictionVector = normalPerpendicular * friction * Vector2.Dot(normalPerpendicular, velocity);

        velocity -= frictionVector;

        var dotProduct = Vector2.Dot(velocity, this._system.Gravity.Perpendicular);
        if (Math.Abs(dotProduct) < this._system.MinimumPostFrictionMagnitude) {
            velocity -= this._system.Gravity.Perpendicular * dotProduct;
        }

        return velocity;
    }

    private Vector2 GetNewVelocity(IDynamicPhysicsBody firstBody, IPhysicsBody otherBody, Vector2 normal, Vector2 normalPerpendicular, float timeStep) {
        if (this._system == null) {
            return firstBody.Velocity;
        }

        var stickyDotProduct = Vector2.Dot(firstBody.Velocity.GetNormalized(), normalPerpendicular);

        if (1f - Math.Abs(stickyDotProduct) <= this._system.Stickiness) {
            var velocity = normalPerpendicular * stickyDotProduct * firstBody.Velocity.Length();
            var friction = (firstBody.PhysicsMaterial.Friction + otherBody.PhysicsMaterial.Friction) * 0.5f * timeStep;
            return this.ApplyFriction(velocity, normalPerpendicular, friction);
        }

        var bounceDotProduct = Vector2.Dot(firstBody.Velocity, this._system.Gravity.Direction);
        if (Math.Abs(bounceDotProduct) < this._system.MinimumPostBounceMagnitude) {
            var groundedDotProduct = Vector2.Dot(normalPerpendicular, this._system.Gravity.Direction);
            if (Math.Abs(groundedDotProduct) <= this._system.Groundedness) {
                var velocity = firstBody.Velocity - this._system.Gravity.Direction * bounceDotProduct;
                var friction = (firstBody.PhysicsMaterial.Friction + otherBody.PhysicsMaterial.Friction) * 0.5f * timeStep;
                return this.ApplyFriction(velocity, normalPerpendicular, friction);
            }
            else {
                var bounce = (firstBody.PhysicsMaterial.Bounce + otherBody.PhysicsMaterial.Bounce) * 0.5f;
                var velocity = this.GetReflectedVelocity(firstBody, normal, bounce);
                return velocity + this._system.Gravity.Direction * bounceDotProduct;
            }
        }

        {
            var bounce = (firstBody.PhysicsMaterial.Bounce + otherBody.PhysicsMaterial.Bounce) * 0.5f;
            return this.GetReflectedVelocity(firstBody, normal, bounce);
        }
    }

    private Vector2 GetReflectedVelocity(IDynamicPhysicsBody firstBody, Vector2 normal, float bounce) {
        var reflectedVelocity = Vector2.Reflect(firstBody.Velocity, normal);
        return reflectedVelocity * bounce;
    }
}