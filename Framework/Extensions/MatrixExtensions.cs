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
        /// Converts from a Matrix to a Transform.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>A transform based on the specified matrix.</returns>
        public static Transform ToTransform(this Matrix matrix) {
            return new Transform(matrix);
        }
    }
}