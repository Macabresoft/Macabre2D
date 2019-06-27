namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Interface for games. Honestly, this was only made so testing could work. Destroy this when possible.
    /// </summary>
    public interface IGame : IHierarchicalEngineObject {

        /// <summary>
        /// Gets the asset manager.
        /// </summary>
        /// <value>The asset manager.</value>
        IAssetManager AssetManager { get; }

        /// <summary>
        /// Gets the content manager.
        /// </summary>
        /// <value>The content manager.</value>
        ContentManager Content { get; }

        /// <summary>
        /// Gets the current scene.
        /// </summary>
        /// <value>The current scene.</value>
        IScene CurrentScene { get; }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Gets the game settings.
        /// </summary>
        /// <value>The game settings.</value>
        IGameSettings Settings { get; }

        /// <summary>
        /// Gets the sprite batch.
        /// </summary>
        /// <value>The sprite batch.</value>
        SpriteBatch SpriteBatch { get; }
    }
}