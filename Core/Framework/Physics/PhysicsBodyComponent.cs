namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base for components that implement <see cref="IPhysicsBody" />.
    /// </summary>
    public abstract class PhysicsBodyComponent : GameComponent, IPhysicsBody {

        /// <summary>
        /// The empty physics body.
        /// </summary>
        internal static readonly IPhysicsBody Empty = new EmptyPhysicsBody();

        [DataMember]
        private int _updateOrder;

        /// <inheritdoc />
        public event EventHandler<CollisionEventArgs>? CollisionOccured;

        /// <inheritdoc />
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc />
        public abstract bool HasCollider { get; }

        /// <inheritdoc />
        [DataMember(Order = 1, Name = "Use as Trigger")]
        public bool IsTrigger { get; set; }

        /// <inheritdoc />
        [DataMember(Order = 2, Name = "Physics Material")]
        public PhysicsMaterial PhysicsMaterial { get; set; } = PhysicsMaterial.Default;

        /// <inheritdoc />
        public int UpdateOrder {
            get {
                return this._updateOrder;
            }

            set {
                this.Set(ref this._updateOrder, value);
            }
        }

        /// <inheritdoc />
        public abstract IEnumerable<Collider> GetColliders();

        /// <inheritdoc />
        public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
            this.CollisionOccured.SafeInvoke(this, eventArgs);
        }

        private sealed class EmptyPhysicsBody : IPhysicsBody {

            /// <inheritdoc />
            public event EventHandler<CollisionEventArgs>? CollisionOccured;

            /// <inheritdoc />
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <inheritdoc />
            public BoundingArea BoundingArea => throw new NotImplementedException();

            /// <inheritdoc />
            public IGameEntity Entity => GameEntity.Empty;

            /// <inheritdoc />
            public bool HasCollider => false;

            /// <inheritdoc />
            public Guid Id => Guid.Empty;

            /// <inheritdoc />
            public bool IsEnabled => false;

            /// <inheritdoc />
            public bool IsTrigger => false;

            /// <inheritdoc />
            public PhysicsMaterial PhysicsMaterial => PhysicsMaterial.Empty;

            /// <inheritdoc />
            public int UpdateOrder => 0;

            /// <inheritdoc />
            public void Dispose() {
                return;
            }

            /// <inheritdoc />
            public IEnumerable<Collider> GetColliders() {
                return new Collider[0];
            }

            /// <inheritdoc />
            public void Initialize(IGameEntity entity) {
                return;
            }

            /// <inheritdoc />
            public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
                return;
            }
        }
    }
}