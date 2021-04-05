namespace Macabresoft.Macabre2D.Framework {

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// A body to be used by the physics engine.
    /// </summary>
    [Display(Name = "Simple Physics Body")]
    public class SimplePhysicsBodyComponent : PhysicsBodyComponent, IPhysicsBodyComponent {
        private Collider? _collider;

        /// <inheritdoc />
        public override BoundingArea BoundingArea {
            get {
                return Collider?.BoundingArea ?? new BoundingArea();
            }
        }

        /// <summary>
        /// Gets the colliders.
        /// </summary>
        /// <value>The colliders.</value>
        [DataMember(Order = 0)]
        public Collider? Collider {
            get {
                return this._collider;
            }

            set {
                if (this.Set(ref this._collider, value)) {
                    this._collider?.Initialize(this);
                }
            }
        }

        /// <inheritdoc />
        public override bool HasCollider {
            get {
                return this.Collider != null;
            }
        }

        /// <inheritdoc />
        public override IEnumerable<Collider> GetColliders() {
            return this.Collider != null ? new[] { this.Collider } : new Collider[0];
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this._collider?.Initialize(this);
        }

        /// <inheritdoc />
        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this._collider?.Reset();
            }
        }
    }
}