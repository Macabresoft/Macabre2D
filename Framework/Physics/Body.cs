namespace Macabre2D.Framework.Physics {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The type of body. This lets the physics engine know what to do about updating this body.
    /// </summary>
    public enum BodyType {

        /// <summary>
        /// The static body type. Will not move or react to physics.
        /// </summary>
        Static = 0,

        /// <summary>
        /// The dynamic body type. The body will react to other bodies, but the physics engine will
        /// not take control of the body's velocity.
        /// </summary>
        Dynamic = 1,

        /// <summary>
        /// The kinematic body type. The body will react to other bodies and the physics engine will
        /// handle its velocity at all times.
        /// </summary>
        Kinematic = 2
    }

    /// <summary>
    /// A body to be used by the physics engine.
    /// </summary>
    /// <seealso cref="BaseComponent"/>
    public class Body : BaseComponent, IBoundable {

        [DataMember]
        private Collider _collider;

        [DataMember]
        private PhysicsMaterial _physicsMaterial = PhysicsMaterial.Default;

        /// <summary>
        /// Occurs when [collision occured].
        /// </summary>
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

        /// <summary>
        /// Gets or sets the physics material.
        /// </summary>
        /// <value>The physics material.</value>
        public PhysicsMaterial PhysicsMaterial {
            get {
                return this._physicsMaterial;
            }

            set {
                this._physicsMaterial = value;
            }
        }

        /// <summary>
        /// Raises the collision occured event.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="CollisionEventArgs"/> instance containing the event data.
        /// </param>
        internal void RaiseCollisionOccured(CollisionEventArgs eventArgs) {
            this.CollisionOccured.SafeInvoke(this, eventArgs);
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;
            this._collider?.Initialize(this);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._collider?.Reset();
        }
    }
}