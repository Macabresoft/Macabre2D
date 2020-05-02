namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.ComponentModel;

    /// <summary>
    /// Interface for a class that contains a world <see cref="Transform"/>.
    /// </summary>
    public interface IWorldTransformable : INotifyPropertyChanged, IScaleable, ITranslateable {

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <value>The world transform.</value>
        Transform WorldTransform { get; }

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(float rotationAngle);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset, float rotationAngle);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <param name="overrideScale">An override value for scale.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, float rotationAngle);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <param name="overrideScale">An override value for scale.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <remarks>This is to be used in conjunction with a <see cref="TileGrid"/>.</remarks>
        /// <param name="grid">The grid.</param>
        /// <param name="gridTileLocation">
        /// The grid tile location. This is the (x, y) coordinate on the grid for which one is
        /// getting the world transform.
        /// </param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(TileGrid grid, Point gridTileLocation);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <remarks>This is to be used in conjunction with a <see cref="TileGrid"/>.</remarks>
        /// <param name="grid">The grid.</param>
        /// <param name="gridTileLocation">
        /// The grid tile location. This is the (x, y) coordinate on the grid for which one is
        /// getting the world transform.
        /// </param>
        /// <param name="offset">The offset.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <remarks>This is to be used in conjunction with a <see cref="TileGrid"/>.</remarks>
        /// <param name="grid">The grid.</param>
        /// <param name="gridTileLocation">
        /// The grid tile location. This is the (x, y) coordinate on the grid for which one is
        /// getting the world transform.
        /// </param>
        /// <param name="offset">The offset.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset, float rotationAngle);

        /// <summary>
        /// Sets the world transform.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        void SetWorldTransform(Vector2 position, Vector2 scale);
    }
}