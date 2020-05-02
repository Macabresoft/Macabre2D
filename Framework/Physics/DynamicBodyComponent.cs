namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// A dynamic body.
    /// </summary>
    public sealed class DynamicBodyComponent : SimpleBodyComponent, IDynamicPhysicsBody {
        private bool _isKinematic;
        private float _mass = 1f;
        private Vector2 _velocity;

        /// <inheritdoc/>
        [DataMember(Name = "Kinematic")]
        public bool IsKinematic {
            get {
                return this._isKinematic;
            }

            set {
                this.Set(ref this._isKinematic, value);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public float Mass {
            get {
                return this._mass;
            }

            set {
                this.Set(ref this._mass, value);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Vector2 Velocity {
            get {
                return this._velocity;
            }

            set {
                this.Set(ref this._velocity, value);
            }
        }
    }
}