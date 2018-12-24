namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Extensions to the Matrix class.
    /// </summary>
    public static class MatrixExtensions {

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