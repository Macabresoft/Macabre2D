namespace Macabre2D.Framework.Rendering {

    /// <summary>
    /// Interface for objects which can render a sprite using a <see cref="SpriteRenderer"/>
    /// </summary>
    public interface ISpriteRenderable {

        /// <summary>
        /// Gets the sprite renderer.
        /// </summary>
        /// <value>The sprite renderer.</value>
        SpriteRenderer SpriteRenderer { get; }
    }
}