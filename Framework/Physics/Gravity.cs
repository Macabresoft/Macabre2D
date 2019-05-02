namespace Macabre2D.Framework.Physics {

    using Macabre2D.Framework.Extensions;
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents gravity in the physics engine. This class helps deal with commonly required things
    /// like the gravity direction and perpindicular.
    /// </summary>
    [DataContract]
    public sealed class Gravity {
        private Vector2 _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gravity"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Gravity(Vector2 value) {
            this.SetProperties(value);
        }

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Vector2>> ValueChanged;

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
                if (this._value != value) {
                    var oldValue = this._value;
                    this.SetProperties(value);
                    this.ValueChanged.SafeInvoke(this, new ValueChangedEventArgs<Vector2>(oldValue, this._value));
                }
            }
        }

        private void SetProperties(Vector2 value) {
            this._value = value;
            this.Direction = this._value.GetNormalized();
            this.Perpindicular = this.Direction.GetPerpendicular();
        }
    }
}