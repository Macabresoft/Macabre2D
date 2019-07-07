namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents an object that can be translated in both world and local space.
    /// </summary>
    public interface ITranslateable {

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        /// <value>The local position.</value>
        Vector2 LocalPosition { get; set; }

        /// <summary>
        /// Sets the world position.
        /// </summary>
        /// <param name="position">The position.</param>
        void SetWorldPosition(Vector2 position);
    }
}