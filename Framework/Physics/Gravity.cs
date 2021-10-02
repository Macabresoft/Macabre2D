namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents gravity in the physics engine. This class helps deal with commonly required
    /// things like the gravity direction and perpendicular.
    /// </summary>
    [DataContract]
    [Category(CommonCategories.Physics)]
    public sealed class Gravity : NotifyPropertyChanged {
        private Vector2 _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gravity" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Gravity(Vector2 value) {
            this._value = value;
            this.SetProperties();
        }

        private Gravity() {
        }

        /// <summary>
        /// Gets the direction of gravity. This is a normalized vector.
        /// </summary>
        /// <value>The direction.</value>
        public Vector2 Direction { get; private set; }

        /// <summary>
        /// Gets the perpendicular of the gravity's direction. This is a normalized vector.
        /// </summary>
        /// <value>The perpendicular.</value>
        public Vector2 Perpendicular { get; private set; }

        /// <summary>
        /// Gets or sets the vector value of this gravity.
        /// </summary>
        /// <value>The value.</value>
        [DataMember(Name = "Gravity")]
        public Vector2 Value {
            get => this._value;

            set {
                if (this.Set(ref this._value, value, true)) {
                    this.SetProperties();
                }
            }
        }

        private void SetProperties() {
            this.Direction = this._value.GetNormalized();
            this.Perpendicular = this.Direction.GetPerpendicular();
        }
    }
}