namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// A <see cref="Transform"/> that also has a <see cref="Rotation"/> attached.
    /// </summary>
    public sealed class RotatableTransform : Transform, IRotatable {

        [DataMember]
        private Rotation _rotation;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotatableTransform"/> class.
        /// </summary>
        public RotatableTransform() : base() {
            this._rotation = new Rotation();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotatableTransform"/> class.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public RotatableTransform(Matrix matrix) {
            var decomposedMatrix = matrix.DecomposeWithRotation2D();
            this.SetPositionAndScale(decomposedMatrix.Position, decomposedMatrix.Scale);
            this._rotation = new Rotation(decomposedMatrix.RotationAngle);
            this._rotation.AngleChanged += this.Rotation_AngleChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotatableTransform"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        public RotatableTransform(Vector2 position, Vector2 scale) : this(position, scale, 0f) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotatableTransform"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        public RotatableTransform(Vector2 position, Vector2 scale, float rotationAngle) : base(position, scale) {
            this._rotation = new Rotation(rotationAngle);
        }

        /// <inheritdoc/>
        public Rotation Rotation {
            get {
                return this._rotation;
            }
        }

        /// <inheritdoc/>
        public override void UpdateTransform(Matrix matrix) {
            var decomposedMatrix = matrix.DecomposeWithRotation2D();
            this.SetPositionAndScale(decomposedMatrix.Position, decomposedMatrix.Scale);
            this.Rotation.Angle = decomposedMatrix.RotationAngle;
        }

        private void Rotation_AngleChanged(object sender, System.EventArgs e) {
            this.RaiseTransformChanged();
        }
    }
}