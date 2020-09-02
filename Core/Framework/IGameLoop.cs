namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Interface for the base object running the update loop for a game.
    /// </summary>
    public interface IGameLoop {

        /// <summary>
        /// Occurs when the game speed has changed.
        /// </summary>
        event EventHandler<double> GameSpeedChanged;

        /// <summary>
        /// Occurs when the viewport changes.
        /// </summary>
        event EventHandler<Point> ViewportSizeChanged;

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
        /// Gets a value indicating whether this game is running in design mode. When the game is
        /// running for real, this value will be false; however, if the game editor is running, it
        /// will be true.
        /// </summary>
        /// <value><c>true</c> if this game is in design mode; otherwise, <c>false</c>.</value>
        bool IsDesignMode { get; }

        /// <summary>
        /// Gets the save data manager.
        /// </summary>
        /// <value>The save data manager.</value>
        ISaveDataManager SaveDataManager { get; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        IGameScene Scene { get; }

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
        /// Gets the size of the viewport.
        /// </summary>
        /// <value>The size of the viewport.</value>
        Point ViewportSize { get; }

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
        void LoadScene(IGameScene scene);

        /// <summary>
        /// Saves the applies graphics settings.
        /// </summary>
        void SaveAndApplyGraphicsSettings();
    }
}