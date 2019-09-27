using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Macabre2D.Framework {

    public abstract class BaseBody : BaseComponent, IPhysicsBody {

        /// <inheritdoc/>
        public event EventHandler<CollisionEventArgs> CollisionOccured;

        /// <inheritdoc/>
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc/>
        public abstract bool HasCollider { get; }

        /// <inheritdoc/>

        /// <inheritdoc/>
        [DataMember(Order = 1)]
        public bool IsTrigger { get; set; }

        /// <inheritdoc/>
        [DataMember(Order = 2)]
        public PhysicsMaterial PhysicsMaterial { get; set; } = PhysicsMaterial.Default;

        public abstract IEnumerable<Collider> GetColliders();

        /// <inheritdoc/>
        public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
            this.CollisionOccured.SafeInvoke(this, eventArgs);
        }
    }
}