namespace Macabre2D.Framework.Extensions {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Extensions to the Matrix class.
    /// </summary>
    public static class MatrixExtensions {

        /// <summary>
        /// Decomposes this matrix to position and scale elements. Returns <c>true</c> if matrix can
        /// be decomposed; <c>false</c> otherwise.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The position and scale.</returns>
        public static (Vector2 Position, Vector2 Scale) Decompose2D(this Matrix matrix) {
            var position = new Vector2(matrix.M41, matrix.M42);

            var xSign = (Math.Sign(matrix.M11) < 0) ? -1 : 1;
            var ySign = (Math.Sign(matrix.M22) < 0) ? -1 : 1;

            var scale = new Vector2(
                xSign * (float)Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13),
                ySign * (float)Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23));

            return (position, scale);
        }

        /// <summary>
        /// Decomposes this matrix to position, scale, and rotation elements.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The position, scale, and rotation angle.</returns>
        public static (Vector2 Position, Vector2 Scale, float RotationAngle) DecomposeWithRotation2D(this Matrix matrix) {
            var originalDecomposedMatrix = matrix.Decompose2D();

            Quaternion quaternion;
            if (originalDecomposedMatrix.Scale.X == 0.0 || originalDecomposedMatrix.Scale.Y == 0.0) {
                quaternion = Quaternion.Identity;
            }
            else {
                var quaternionMatrix = new Matrix(
                    matrix.M11 / originalDecomposedMatrix.Scale.X, matrix.M12 / originalDecomposedMatrix.Scale.X, matrix.M13 / originalDecomposedMatrix.Scale.X, 0,
                    matrix.M21 / originalDecomposedMatrix.Scale.Y, matrix.M22 / originalDecomposedMatrix.Scale.Y, matrix.M23 / originalDecomposedMatrix.Scale.Y, 0,
                    matrix.M31, matrix.M32, matrix.M33, 0,
                    0, 0, 0, 1);

                quaternion = Quaternion.CreateFromRotationMatrix(quaternionMatrix);
            }

            var direction = Vector2.Transform(Vector2.UnitX, quaternion);
            var rotationAngle = (float)Math.Atan2(direction.Y, direction.X);

            return (originalDecomposedMatrix.Position, originalDecomposedMatrix.Scale, rotationAngle);
        }

        /// <summary>
        /// Converts from a <see cref="Matrix"/> to a <see cref="RotatableTransform"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>A rotatable transform based on the specified matrix.</returns>
        public static RotatableTransform ToRotatableTransform(this Matrix matrix) {
            return new RotatableTransform(matrix);
        }

        /// <summary>
        /// Converts from a <see cref="Matrix"/> to a <see cref="Transform"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>A transform based on the specified matrix.</returns>
        public static Transform ToTransform(this Matrix matrix) {
            return new Transform(matrix);
        }
    }
}