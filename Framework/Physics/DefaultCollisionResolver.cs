namespace Macabre2D.Framework.Physics {

    using Macabre2D.Framework.Extensions;
    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// The default collision resolver. This will be used
    /// </summary>
    /// <seealso cref="ICollisionResolver"/>
    public sealed class DefaultCollisionResolver : ICollisionResolver {
        private ICollisionBasedPhysicsModule _module;

        /// <inheritdoc/>
        public void Initialize(ICollisionBasedPhysicsModule module) {
            this._module = module;
        }

        /// <inheritdoc/>
        public void ResolveCollision(CollisionEventArgs e, float timeStep) {
            if (e.FirstCollider.Body is DynamicBody firstBody) {
                firstBody.SetWorldPosition(firstBody.WorldTransform.Position + e.MinimumTranslationVector);

                if (firstBody.IsKinematic) {
                    var normalPerpindicular = e.Normal.GetPerpendicular();

                    if (e.SecondCollider.Body is DynamicBody otherBody && otherBody.IsKinematic) {
                        var firstBodyMagnitude = firstBody.Velocity.Length();
                        var otherBodyMagnitude = otherBody.Velocity.Length();

                        var firstMassMultiplier = firstBody.Mass * firstBodyMagnitude;
                        var secondMassMultipler = otherBody.Mass * otherBodyMagnitude;

                        var totalMass = firstMassMultiplier + secondMassMultipler;
                        var massRatio = totalMass == 0f ? 0.5f : firstMassMultiplier / totalMass;
                        var inverseMassRatio = 1f - massRatio;

                        var bounce = (firstBody.PhysicsMaterial.Bounce + otherBody.PhysicsMaterial.Bounce) * 0.5f;
                        var firstReflection = this.GetReflectedVelocity(firstBody, otherBody, e.Normal, bounce);
                        var otherReflection = this.GetReflectedVelocity(otherBody, firstBody, e.Normal, bounce);

                        var averageMagnitude = (firstBodyMagnitude + otherBodyMagnitude) * 0.5f;
                        firstBody.Velocity = massRatio * firstReflection + averageMagnitude * inverseMassRatio * e.Normal;
                        otherBody.Velocity = inverseMassRatio * otherReflection + averageMagnitude * massRatio * -e.Normal;
                    }
                    else {
                        firstBody.Velocity = this.GetNewVelocity(firstBody, e.SecondCollider.Body, e.Normal, normalPerpindicular, timeStep);
                    }
                }
            }
        }

        private Vector2 ApplyFriction(Vector2 velocity, Vector2 normalPerpindicular, float friction) {
            if (friction == 0) {
                return velocity;
            }

            var frictionVector = normalPerpindicular * friction * Vector2.Dot(normalPerpindicular, velocity);

            velocity -= frictionVector;

            var dotProduct = Vector2.Dot(velocity, this._module.Gravity.Perpindicular);
            if (Math.Abs(dotProduct) < this._module.MinimumPostFrictionMagnitude) {
                velocity -= this._module.Gravity.Perpindicular * dotProduct;
            }

            return velocity;
        }

        private Vector2 GetNewVelocity(DynamicBody firstBody, Body otherBody, Vector2 normal, Vector2 normalPerpindicular, float timeStep) {
            var stickyDotProduct = Vector2.Dot(firstBody.Velocity.GetNormalized(), normalPerpindicular);

            if (1f - Math.Abs(stickyDotProduct) <= this._module.Stickiness) {
                var velocity = normalPerpindicular * stickyDotProduct * firstBody.Velocity.Length();
                var friction = (firstBody.PhysicsMaterial.Friction + otherBody.PhysicsMaterial.Friction) * 0.5f * timeStep;
                return this.ApplyFriction(velocity, normalPerpindicular, friction);
            }
            else {
                var bounceDotProduct = Vector2.Dot(firstBody.Velocity, this._module.Gravity.Direction);
                if (Math.Abs(bounceDotProduct) < this._module.MinimumPostBounceMagnitude) {
                    var groundedDotProduct = Vector2.Dot(normalPerpindicular, this._module.Gravity.Direction);
                    if (Math.Abs(groundedDotProduct) <= this._module.Groundedness) {
                        var velocity = firstBody.Velocity - this._module.Gravity.Direction * bounceDotProduct;
                        var friction = (firstBody.PhysicsMaterial.Friction + otherBody.PhysicsMaterial.Friction) * 0.5f * timeStep;
                        return this.ApplyFriction(velocity, normalPerpindicular, friction);
                    }
                    else {
                        var bounce = (firstBody.PhysicsMaterial.Bounce + otherBody.PhysicsMaterial.Bounce) * 0.5f;
                        var velocity = this.GetReflectedVelocity(firstBody, otherBody, normal, bounce);
                        return velocity + this._module.Gravity.Direction * bounceDotProduct;
                    }
                }
                else {
                    var bounce = (firstBody.PhysicsMaterial.Bounce + otherBody.PhysicsMaterial.Bounce) * 0.5f;
                    return this.GetReflectedVelocity(firstBody, otherBody, normal, bounce);
                }
            }
        }

        private Vector2 GetReflectedVelocity(DynamicBody firstBody, Body otherBody, Vector2 normal, float bounce) {
            var reflectedVelocity = Vector2.Reflect(firstBody.Velocity, normal);
            return reflectedVelocity * bounce;
        }
    }
}