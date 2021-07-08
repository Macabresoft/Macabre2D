namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Extensions to the Matrix class.
    /// </summary>
    public static class MatrixExtensions {

        /// <summary>
        /// Decomposes this matrix to position, scale, and rotation in a <see cref="Transform"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The transform.</returns>
        public static Transform Decompose2D(this Matrix matrix) {
            var position = new Vector2(matrix.M41, matrix.M42);

            var xSign = (Math.Sign(matrix.M11) < 0) ? -1 : 1;
            var ySign = (Math.Sign(matrix.M22) < 0) ? -1 : 1;

            var scale = new Vector2(
                xSign * (float)Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13),
                ySign * (float)Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23));

            Quaternion quaternion;
            if (scale.X == 0.0 || scale.Y == 0.0) {
                quaternion = Quaternion.Identity;
            }
            else {
                var quaternionMatrix = new Matrix(
                    matrix.M11 / scale.X, matrix.M12 / scale.X, matrix.M13 / scale.X, 0,
                    matrix.M21 / scale.Y, matrix.M22 / scale.Y, matrix.M23 / scale.Y, 0,
                    matrix.M31, matrix.M32, matrix.M33, 0,
                    0, 0, 0, 1);

                quaternion = Quaternion.CreateFromRotationMatrix(quaternionMatrix);
            }

            var direction = Vector2.Transform(Vector2.UnitX, quaternion);
            var rotationAngle = (float)Math.Atan2(direction.Y, direction.X);

            return new Transform(position, scale, rotationAngle);
        }

        /// <summary>
        /// Decomposes this matrix to position and scale in a <see cref="Transform"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The transform.</returns>
        public static Transform DecomposeWithoutRotation2D(this Matrix matrix) {
            var position = new Vector2(matrix.M41, matrix.M42);

            var xSign = (Math.Sign(matrix.M11) < 0) ? -1 : 1;
            var ySign = (Math.Sign(matrix.M22) < 0) ? -1 : 1;

            var scale = new Vector2(
                xSign * (float)Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13),
                ySign * (float)Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23));

            return new Transform(position, scale);
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