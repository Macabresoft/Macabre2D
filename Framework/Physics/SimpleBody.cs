namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A body to be used by the physics engine.
    /// </summary>
    /// <seealso cref="BaseComponent"/>
    public class SimpleBody : BaseComponent, IPhysicsBody {
        private Collider _collider;

        [DataMember]
        private PhysicsMaterial _physicsMaterial = PhysicsMaterial.Default;

        /// <inheritdoc/>
        public event EventHandler<CollisionEventArgs> CollisionOccured;

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this.Collider != null ? this.Collider.BoundingArea : new BoundingArea();
            }
        }

        /// <summary>
        /// Gets the colliders.
        /// </summary>
        /// <value>The colliders.</value>
        [DataMember]
        public Collider Collider {
            get {
                return this._collider;
            }

            set {
                this._collider = value;

                if (this.IsInitialized && this._collider != null) {
                    this._collider.Initialize(this);
                }
            }
        }

        /// <inheritdoc/>
        public bool HasCollider {
            get {
                return this.Collider != null;
            }
        }

        /// <inheritdoc/>
        public PhysicsMaterial PhysicsMaterial {
            get {
                return this._physicsMaterial;
            }

            set {
                this._physicsMaterial = value;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Collider> GetColliders() {
            return this.Collider != null ? new[] { this.Collider } : new Collider[0];
        }

        /// <inheritdoc/>
        public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
            this.CollisionOccured.SafeInvoke(this, eventArgs);
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged -= this.Self_TransformChanged;
            this.TransformChanged += this.Self_TransformChanged;
            this._collider?.Initialize(this);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._collider?.Reset();
        }
    }
}