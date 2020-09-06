namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents gravity in the physics engine. This class helps deal with commonly required
    /// things like the gravity direction and perpindicular.
    /// </summary>
    [DataContract]
    public sealed class Gravity : NotifyPropertyChanged {

        /// <summary>
        /// Empty gravity.
        /// </summary>
        public static readonly Gravity Empty = new Gravity();

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
        /// Gets the perpindicular of the gravity's direction. This is a normalized vector.
        /// </summary>
        /// <value>The perpindicular.</value>
        public Vector2 Perpindicular { get; private set; }

        /// <summary>
        /// Gets or sets the vector value of this gravity.
        /// </summary>
        /// <value>The value.</value>
        [DataMember]
        public Vector2 Value {
            get {
                return this._value;
            }

            set {
                if (this.Set(ref this._value, value, true)) {
                    this.SetProperties();
                }
            }
        }

        private void SetProperties() {
            this.Direction = this._value.GetNormalized();
            this.Perpindicular = this.Direction.GetPerpendicular();
        }
    }
}