namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Interface for games. Honestly, this was only made so testing could work. Destroy this when possible.
    /// </summary>
    public interface IGame {

        /// <summary>
        /// Occurs when the game speed has changed.
        /// </summary>
        event EventHandler<double> GameSpeedChanged;

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
        /// Gets or sets the game speed.
        /// </summary>
        /// <value>The game speed.</value>
        double GameSpeed { get; set; }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Gets the graphics settings.
        /// </summary>
        /// <value>The graphics settings.</value>
        GraphicsSettings GraphicsSettings { get; }

        /// <summary>
        /// Gets a value indicating whether this game should instantiate prefabs.
        /// </summary>
        /// <value><c>true</c> if this game should instantiate prefabs; otherwise, <c>false</c>.</value>
        bool InstantiatePrefabs { get; }

        /// <summary>
        /// Gets the save data manager.
        /// </summary>
        /// <value>The save data manager.</value>
        ISaveDataManager SaveDataManager { get; }

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

        /// <summary>
        /// Exits thie game.
        /// </summary>
        void Exit();

        /// <summary>
        /// Saves the applies graphics settings.
        /// </summary>
        void SaveAndApplyGraphicsSettings();
    }
}