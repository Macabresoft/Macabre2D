namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents transform information for an object.
    /// </summary>
    [DataContract]
    public sealed class Transform {

        [DataMember]
        private Vector2 _position;

        [DataMember]
        private Rotation _rotation;

        [DataMember]
        private Vector2 _scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class.
        /// </summary>
        public Transform() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> struct.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public Transform(Matrix matrix) {
            Vector3 position;
            Quaternion rotation;
            Vector3 scale;

            matrix.Decompose(out scale, out rotation, out position);

            this._scale = scale.ToVector2();
            this._position = position.ToVector2();

            var direction = Vector2.Transform(Vector2.UnitX, rotation);
            this._rotation = new Rotation((float)Math.Atan2(direction.Y, direction.X));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="angle">The angle of the rotation.</param>
        /// <param name="scale">The scale.</param>
        public Transform(Vector2 position, float angle, Vector2 scale) : this(position, new Rotation(angle), scale) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        public Transform(Vector2 position, Rotation rotation, Vector2 scale) {
            this._position = position;
            this._rotation = rotation;
            this._scale = scale;
        }

        /// <summary>
        /// Occurs when [transform changed].
        /// </summary>
        public event EventHandler TransformChanged;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector2 Position {
            get {
                return this._position;
            }

            set {
                if (this._position != value) {
                    this._position = value;
                    this.TransformChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Rotation Rotation {
            get {
                return this._rotation;
            }
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public Vector2 Scale {
            get {
                return this._scale;
            }

            set {
                if (this._scale != value) {
                    this._scale = value;
                    this.TransformChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Updates the transform.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="scale">The scale.</param>
        public void UpdateTransform(Vector2 position, float angle, Vector2 scale) {
            this._position = position;
            this._rotation = new Rotation(angle);
            this._scale = scale;

            this.TransformChanged.SafeInvoke(this);
        }

        /// <summary>
        /// Updates the transform.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void UpdateTransform(Matrix matrix) {
            matrix.Decompose(out var scale, out var rotation, out var position);

            this._scale = scale.ToVector2();
            this._position = position.ToVector2();

            var direction = Vector2.Transform(Vector2.UnitX, rotation);
            this._rotation = new Rotation((float)Math.Atan2(direction.Y, direction.X));

            this.TransformChanged.SafeInvoke(this);
        }
    }
}