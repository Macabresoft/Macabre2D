namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A module which allows for raycasting and handles rigidbody physics interactions.
    /// </summary>
    /// <seealso cref="SimplePhysicsModule"/>
    public sealed class PhysicsModule : SimplePhysicsModule, ICollisionBasedPhysicsModule {
        private readonly Dictionary<int, List<int>> _collisionsHandled = new Dictionary<int, List<int>>();

        private ICollisionResolver _collisionResolver;
        private float _groundedness = 0.35f;
        private float _minimumPostBounceMagnitude = 1.5f;
        private float _minimumPostFrictionMagnitude = 0.2f;
        private float _stickiness = 0.1f;

        [DataMember(Name = "Collision Resolver", Order = 0)]
        public ICollisionResolver CollisionResolver {
            get {
                return this._collisionResolver;
            }

            private set {
                this.Set(ref this._collisionResolver, value);
            }
        }

        /// <inheritdoc/>
        [DataMember(Order = 1)]
        public Gravity Gravity { get; } = new Gravity(Vector2.Zero);

        /// <inheritdoc/>
        [DataMember(Order = 2)]
        public float Groundedness {
            get {
                return this._groundedness;
            }

            set {
                this.Set(ref this._groundedness, value);
            }
        }

        /// <inheritdoc/>
        [DataMember(Order = 4, Name = "Minimum Post-Bounce Magnitude")]
        public float MinimumPostBounceMagnitude {
            get {
                return this._minimumPostBounceMagnitude;
            }

            set {
                this.Set(ref this._minimumPostBounceMagnitude, value);
            }
        }

        /// <inheritdoc/>
        [DataMember(Order = 5, Name = "Minimum Post-Friction Magnitude")]
        public float MinimumPostFrictionMagnitude {
            get {
                return this._minimumPostFrictionMagnitude;
            }

            set {
                this.Set(ref this._minimumPostFrictionMagnitude, value);
            }
        }

        /// <inheritdoc/>
        [DataMember(Order = 3)]
        public float Stickiness {
            get {
                return this._stickiness;
            }

            set {
                this.Set(ref this._stickiness, value);
            }
        }

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
                    foreach (var otherCollider in potentials.Where(c => c != collider && GameSettings.Instance.Layers.GetShouldCollide(c.Layers, collider.Layers))) {
                        var hasCollisionAlreadyResolved = this._collisionsHandled.TryGetValue(otherCollider.Body.SessionId, out var collisions) &&
                            collisions.Contains(body.SessionId);

                        if (!hasCollisionAlreadyResolved && collider.CollidesWith(otherCollider, out var collision)) {
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
                            collisionsOccured.Add(otherCollider.Body.SessionId);
                        }
                    }

                    this._collisionsHandled[body.SessionId] = collisionsOccured;
                }
            }
        }
    }
}