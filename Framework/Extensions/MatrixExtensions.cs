namespace Macabre2D.Framework.Extensions {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Extensions to the Matrix class.
    /// </summary>
    public static class MatrixExtensions {

        /// <summary>
        /// Decomposes this matrix to translation, rotation and scale elements. Returns <c>true</c>
        /// if matrix can be decomposed; <c>false</c> otherwise.
        /// </summary>
        /// <param name="scale">Scale vector as an output parameter.</param>
        /// <param name="rotation">Rotation quaternion as an output parameter.</param>
        /// <param name="translation">Translation vector as an output parameter.</param>
        /// <returns><c>true</c> if matrix can be decomposed; <c>false</c> otherwise.</returns>
        public static bool Decompose2D(this Matrix matrix, out Vector2 scale, out Quaternion rotation, out Vector2 translation) {
            translation = new Vector2(matrix.M41, matrix.M42);

            var xSign = (Math.Sign(matrix.M11) < 0) ? -1 : 1;
            var ySign = (Math.Sign(matrix.M22) < 0) ? -1 : 1;

            scale = new Vector2(
                xSign * (float)Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13),
                ySign * (float)Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23));

            var result = true;
            if (scale.X == 0f || scale.Y == 0f) {
                rotation = Quaternion.Identity;
                result = false;
            }
            else {
                var rotationMatrix = new Matrix(
                    matrix.M11 / scale.X, matrix.M12 / scale.X, matrix.M13 / scale.X, 0f,
                    matrix.M21 / scale.Y, matrix.M22 / scale.Y, matrix.M23 / scale.Y, 0f,
                    matrix.M31, matrix.M32, matrix.M33, 0f,
                    0f, 0f, 0f, 1f);

                rotation = Quaternion.CreateFromRotationMatrix(rotationMatrix);
            }

            return result;
        }

        /// <summary>
        /// Converts from a Matrix to a Transform.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>A transform based on the specified matrix.</returns>
        public static Transform ToTransform(this Matrix matrix) {
            return new Transform(matrix);
        }
    }
}