namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public abstract class BaseBodyComponent : BaseComponent, IPhysicsBody {

        /// <inheritdoc/>
        public event EventHandler<CollisionEventArgs> CollisionOccured;

        /// <inheritdoc/>
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc/>
        public abstract bool HasCollider { get; }

        /// <inheritdoc/>

        /// <inheritdoc/>
        [DataMember(Order = 1, Name = "Use as Trigger")]
        public bool IsTrigger { get; set; }

        /// <inheritdoc/>
        [DataMember(Order = 2, Name = "Physics Material")]
        public PhysicsMaterial PhysicsMaterial { get; set; } = PhysicsMaterial.Default;

        public abstract IEnumerable<Collider> GetColliders();

        /// <inheritdoc/>
        public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
            this.CollisionOccured.SafeInvoke(this, eventArgs);
        }
    }
}