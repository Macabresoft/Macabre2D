namespace Macabre2D.Framework {

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// A body to be used by the physics engine.
    /// </summary>
    /// <seealso cref="BaseComponent"/>
    public class SimpleBodyComponent : BaseBody, IPhysicsBody {
        private Collider _collider;

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                return this.Collider != null ? this.Collider.BoundingArea : new BoundingArea();
            }
        }

        /// <summary>
        /// Gets the colliders.
        /// </summary>
        /// <value>The colliders.</value>
        [DataMember(Order = 0)]
        public Collider Collider {
            get {
                return this._collider;
            }

            set {
                if (this.Set(ref this._collider, value)) {
                    this._collider?.Initialize(this);
                }
            }
        }

        /// <inheritdoc/>
        public override bool HasCollider {
            get {
                return this.Collider != null;
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<Collider> GetColliders() {
            return this.Collider != null ? new[] { this.Collider } : new Collider[0];
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.PropertyChanged -= this.Self_PropertyChanged;
            this.PropertyChanged += this.Self_PropertyChanged;
            this._collider?.Initialize(this);
        }

        private void Self_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.WorldTransform)) {
                this._collider?.Reset();
            }
        }
    }
}