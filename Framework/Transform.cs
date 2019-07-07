namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents transform information for an object.
    /// </summary>
    [DataContract]
    public class Transform {

        [DataMember]
        private Vector2 _position;

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
            var decomposedMatrix = matrix.Decompose2D();

            this._scale = decomposedMatrix.Scale;
            this._position = decomposedMatrix.Position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        public Transform(Vector2 position, Vector2 scale) {
            this._position = position;
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
                    this.RaiseTransformChanged();
                }
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
                    this.RaiseTransformChanged();
                }
            }
        }

        /// <summary>
        /// Updates the transform.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        public void UpdateTransform(Vector2 position, Vector2 scale) {
            this._position = position;
            this._scale = scale;

            this.RaiseTransformChanged();
        }

        /// <summary>
        /// Updates the transform.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public virtual void UpdateTransform(Matrix matrix) {
            var decomposedMatrix = matrix.Decompose2D();
            this._scale = decomposedMatrix.Scale;
            this._position = decomposedMatrix.Position;
            this.RaiseTransformChanged();
        }

        internal void RaiseTransformChanged() {
            this.TransformChanged.SafeInvoke(this);
        }

        internal void SetPositionAndScale(Vector2 position, Vector2 scale) {
            this._position = position;
            this._scale = scale;
        }
    }
}