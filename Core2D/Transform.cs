namespace Macabresoft.MonoGame.Core2D {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents transform information for an object.
    /// </summary>
    [DataContract]
    public struct Transform {

        /// <summary>
        /// The origin trasnform.
        /// </summary>
        public static readonly Transform Origin = new Transform(Vector2.Zero, Vector2.One);

        [DataMember]
        public readonly Vector2 Position;

        [DataMember]
        public readonly float Rotation;

        [DataMember]
        public readonly Vector2 Scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform" /> struct.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public Transform(Matrix matrix) {
            var decomposedMatrix = matrix.Decompose2D();

            this.Scale = decomposedMatrix.Scale;
            this.Position = decomposedMatrix.Position;
            this.Rotation = decomposedMatrix.Rotation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform" /> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        public Transform(Vector2 position, Vector2 scale) : this(position, scale, 0f) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform" /> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation in radians.</param>
        public Transform(Vector2 position, Vector2 scale, float rotation) {
            this.Position = position;
            this.Scale = scale;
            this.Rotation = rotation.NormalizeAngle();
        }

        public static bool operator !=(Transform left, Transform right) {
            return !(left == right);
        }

        public static bool operator ==(Transform left, Transform right) {
            return left.Equals(right);
        }

        public override bool Equals(object? obj) {
            return obj is Transform transform && this.Position == transform.Position && this.Rotation == transform.Rotation && this.Scale == transform.Scale;
        }

        public override int GetHashCode() {
            return System.HashCode.Combine(this.Position, this.Scale, this.Rotation);
        }
    }
}