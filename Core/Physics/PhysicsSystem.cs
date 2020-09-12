namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A system which allows for raycasting and handles rigidbody physics interactions.
    /// </summary>
    /// <seealso cref="SimplePhysicsSystem" />
    public sealed class PhysicsSystem : SimplePhysicsSystem, IGamePhysicsSystem {
        private readonly Dictionary<Guid, List<Guid>> _collisionsHandled = new Dictionary<Guid, List<Guid>>();

        [DataMember(Name = "Collision Resolver", Order = 0)]
        private ICollisionResolver _collisionResolver = new DefaultCollisionResolver();

        private float _groundedness = 0.35f;
        private float _minimumPostBounceMagnitude = 1.5f;
        private float _minimumPostFrictionMagnitude = 0.2f;
        private float _stickiness = 0.1f;

        /// <summary>
        /// Gets the collision resolver.
        /// </summary>
        /// <value>The collision resolver.</value>
        public ICollisionResolver CollisionResolver {
            get {
                return this._collisionResolver;
            }

            private set {
                this.Set(ref this._collisionResolver, value);
            }
        }

        /// <inheritdoc />
        [DataMember(Order = 1)]
        public Gravity Gravity { get; } = new Gravity(Vector2.Zero);

        /// <inheritdoc />
        [DataMember(Order = 2)]
        public float Groundedness {
            get {
                return this._groundedness;
            }

            set {
                this.Set(ref this._groundedness, value);
            }
        }

        /// <inheritdoc />
        [DataMember(Order = 4, Name = "Minimum Post-Bounce Magnitude")]
        public float MinimumPostBounceMagnitude {
            get {
                return this._minimumPostBounceMagnitude;
            }

            set {
                this.Set(ref this._minimumPostBounceMagnitude, value);
            }
        }

        /// <inheritdoc />
        [DataMember(Order = 5, Name = "Minimum Post-Friction Magnitude")]
        public float MinimumPostFrictionMagnitude {
            get {
                return this._minimumPostFrictionMagnitude;
            }

            set {
                this.Set(ref this._minimumPostFrictionMagnitude, value);
            }
        }

        /// <inheritdoc />
        [DataMember(Order = 3)]
        public float Stickiness {
            get {
                return this._stickiness;
            }

            set {
                this.Set(ref this._stickiness, value);
            }
        }

        /// <inheritdoc />
        public override void Initialize(IGameScene scene) {
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
                dynamicBody.Entity.SetWorldPosition(dynamicBody.Entity.Transform.Position + dynamicBody.Velocity * this.TimeStep);

                if (dynamicBody.IsKinematic) {
                    dynamicBody.Velocity += this.Gravity.Value * this.TimeStep;
                }

                foreach (var collider in body.GetColliders()) {
                    var potentials = this.ColliderTree.RetrievePotentialCollisions(collider);
                    foreach (var otherCollider in potentials.Where(c => c != collider && GameSettings.Instance.Layers.GetShouldCollide(c.Layers, collider.Layers))) {
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
}