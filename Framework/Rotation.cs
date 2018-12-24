namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a rotation for transforms.
    /// </summary>
    [DataContract]
    public sealed class Rotation {
        private float _angle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rotation"/> class.
        /// </summary>
        public Rotation() : this(0f) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rotation"/> struct.
        /// </summary>
        /// <param name="angle">The angle in radians.</param>
        public Rotation(float angle) {
            this.SetAngle(angle);
        }

        /// <summary>
        /// Occurs when [angle changed].
        /// </summary>
        public event EventHandler AngleChanged;

        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        /// <value>The angle.</value>
        [DataMember]
        public float Angle {
            get {
                return this._angle;
            }

            set {
                this.SetAngle(value);
                this.AngleChanged.SafeInvoke(this);
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            return ((Rotation)obj).Angle == this.Angle;
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return new { this.Angle }.GetHashCode();
        }

        private void SetAngle(float value) {
            this._angle = value;

            // Normalize the angle between 0 and TwoPi.
            while (this._angle >= MathHelper.TwoPi) {
                this._angle -= MathHelper.TwoPi;
            }

            while (this._angle < 0) {
                this._angle += MathHelper.TwoPi;
            }
        }
    }
}