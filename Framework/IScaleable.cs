namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents an object that can be scaled in both world and local space.
    /// </summary>
    public interface IScaleable {

        /// <summary>
        /// Gets or sets the local scale.
        /// </summary>
        /// <value>The local scale.</value>
        Vector2 LocalScale { get; set; }

        /// <summary>
        /// Sets the world scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        void SetWorldScale(Vector2 scale);
    }
}