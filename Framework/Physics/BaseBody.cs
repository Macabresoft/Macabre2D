using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Macabre2D.Framework {

    public abstract class BaseBody : BaseComponent, IPhysicsBody {

        [DataMember]
        private PhysicsMaterial _physicsMaterial = PhysicsMaterial.Default;

        /// <inheritdoc/>
        public event EventHandler<CollisionEventArgs> CollisionOccured;

        /// <inheritdoc/>
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc/>
        public abstract bool HasCollider { get; }

        /// <inheritdoc/>

        /// <inheritdoc/>
        public PhysicsMaterial PhysicsMaterial {
            get {
                return this._physicsMaterial;
            }

            set {
                this._physicsMaterial = value;
            }
        }

        public abstract IEnumerable<Collider> GetColliders();

        /// <inheritdoc/>
        public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
            this.CollisionOccured.SafeInvoke(this, eventArgs);
        }
    }
}