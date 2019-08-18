namespace Macabre2D.Framework {

    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A module which allows for raycasting and handles rigidbody physics interactions.
    /// </summary>
    /// <seealso cref="SimplePhysicsModule"/>
    public sealed class PhysicsModule : SimplePhysicsModule, ICollisionBasedPhysicsModule {
        private readonly Dictionary<int, List<int>> _collisionsHandled = new Dictionary<int, List<int>>();

        [DataMember]
        private ICollisionResolver _collisionResolver;

        /// <inheritdoc/>
        [DataMember]
        public Gravity Gravity { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public float Groundedness { get; set; } = 0.35f;

        /// <inheritdoc/>
        [DataMember]
        public float MinimumPostBounceMagnitude { get; set; } = 1.5f;

        /// <inheritdoc/>
        [DataMember]
        public float MinimumPostFrictionMagnitude { get; set; } = 0.2f;

        /// <inheritdoc/>
        [DataMember]
        public float Stickiness { get; set; } = 0.1f;

        /// <inheritdoc/>
        public override void FixedPostUpdate() {
            base.FixedPostUpdate();
            this._collisionsHandled.Clear();
            this.Bodies.ForEachFilteredItem(this.HandleCollisions);
        }

        /// <inheritdoc/>
        public override void PostInitialize() {
            base.PostInitialize();

            if (this._collisionResolver == null) {
                this._collisionResolver = new DefaultCollisionResolver();
            }

            this._collisionResolver.Initialize(this);
        }

        private void HandleCollisions(IPhysicsBody body) {
            if (body.HasCollider && body is IDynamicPhysicsBody dynamicBody) {
                var collisionsOccured = new List<int>();
                dynamicBody.SetWorldPosition(dynamicBody.WorldTransform.Position + dynamicBody.Velocity * this.TimeStep);

                if (dynamicBody.IsKinematic) {
                    dynamicBody.Velocity += this.Gravity.Value * this.TimeStep;
                }

                foreach (var collider in body.GetColliders()) {
                    var potentials = this.ColliderTree.RetrievePotentialCollisions(collider);
                    foreach (var otherCollider in potentials.Where(c => c != collider && (c.Body.Layers & body.Layers) != Layers.None)) {
                        var hasCollisionAlreadyResolved = this._collisionsHandled.TryGetValue(otherCollider.Body.SessionId, out var collisions) &&
                            collisions.Contains(body.SessionId);

                        if (!hasCollisionAlreadyResolved && collider.CollidesWith(otherCollider, out var collision)) {
                            this._collisionResolver.ResolveCollision(collision, this.TimeStep);

                            body.NotifyCollisionOccured(collision);

                            var otherCollision = new CollisionEventArgs(
                                collision.SecondCollider,
                                collision.FirstCollider,
                                collision.Normal,
                                -collision.MinimumTranslationVector,
                                collision.SecondContainsFirst,
                                collision.FirstContainsSecond);

                            otherCollider.Body.NotifyCollisionOccured(otherCollision);
                            collisionsOccured.Add(otherCollider.Body.SessionId);
                        }
                    }

                    this._collisionsHandled[body.SessionId] = collisionsOccured;
                }
            }
        }
    }
}