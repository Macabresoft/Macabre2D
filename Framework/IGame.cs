namespace Macabresoft.Macabre2D.Framework {
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Interface for the base object running the update loop for a game.
    /// </summary>
    public interface IGame {
        /// <summary>
        /// Occurs when the game speed has changed.
        /// </summary>
        event EventHandler<double> GameSpeedChanged;

        /// <summary>
        /// Occurs when the viewport changes.
        /// </summary>
        event EventHandler<Point> ViewportSizeChanged;

        /// <summary>
        /// Gets the assets.
        /// </summary>
        IAssetManager Assets { get; }

        /// <summary>
        /// Gets the content manager.
        /// </summary>
        /// <value>The content manager.</value>
        ContentManager? Content { get; }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        GraphicsDevice? GraphicsDevice { get; }

        /// <summary>
        /// Gets the graphics settings.
        /// </summary>
        /// <value>The graphics settings.</value>
        GraphicsSettings GraphicsSettings { get; }

        /// <summary>
        /// Gets the state of input.
        /// </summary>
        /// <value>The state of input.</value>
        InputState InputState => new();

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <value>The project.</value>
        IGameProject Project { get; }

        /// <summary>
        /// Gets the save data manager.
        /// </summary>
        /// <value>The save data manager.</value>
        ISaveDataManager SaveDataManager { get; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        IScene Scene { get; }

        /// <summary>
        /// Gets the sprite batch.
        /// </summary>
        /// <value>The sprite batch.</value>
        SpriteBatch? SpriteBatch { get; }

        /// <summary>
        /// Gets the size of the viewport.
        /// </summary>
        /// <value>The size of the viewport.</value>
        Point ViewportSize { get; }

        /// <summary>
        /// Gets or sets the game speed.
        /// </summary>
        /// <value>The game speed.</value>
        double GameSpeed { get; set; }

        /// <summary>
        /// Exits thie game.
        /// </summary>
        void Exit();

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        void LoadScene(string sceneName);

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void LoadScene(IScene scene);

        /// <summary>
        /// Saves the applies graphics settings.
        /// </summary>
        void SaveAndApplyGraphicsSettings();

        /// <summary>
        /// Saves the current graphics settings.
        /// </summary>
        void SaveGraphicsSettings();
    }
}