namespace Macabresoft.Macabre2D.Framework;

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
    /// Gets the content manager.
    /// </summary>
    ContentManager? Content { get; }

    /// <summary>
    /// Gets or sets the game speed.
    /// </summary>
    double GameSpeed { get; set; }

    /// <summary>
    /// Gets the graphics device.
    /// </summary>
    GraphicsDevice? GraphicsDevice { get; }

    /// <summary>
    /// Gets the graphics settings.
    /// </summary>
    GraphicsSettings GraphicsSettings { get; }

    /// <summary>
    /// Gets the input bindings.
    /// </summary>
    InputBindings InputBindings { get; }

    /// <summary>
    /// Gets the state of input.
    /// </summary>
    InputState InputState => new();

    /// <summary>
    /// Gets the project.
    /// </summary>
    /// <value>The project.</value>
    IGameProject Project { get; }

    /// <summary>
    /// Gets the save data manager.
    /// </summary>
    ISaveDataManager SaveDataManager { get; }

    /// <summary>
    /// Gets the scene.
    /// </summary>
    IScene CurrentScene { get; }

    /// <summary>
    /// Gets the sprite batch.
    /// </summary>
    SpriteBatch? SpriteBatch { get; }

    /// <summary>
    /// Gets the size of the viewport.
    /// </summary>
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
    void LoadScene(IScene scene);

    /// <summary>
    /// Saves the applies graphics settings.
    /// </summary>
    void SaveAndApplyGraphicsSettings();

    /// <summary>
    /// Saves the current graphics settings.
    /// </summary>
    void SaveGraphicsSettings();

    /// <summary>
    /// Saves the current input bindings.
    /// </summary>
    void SaveInputBindings();

    /// <summary>
    /// Unpauses the game and stops updating and rendering the pause scene.
    /// </summary>
    void Unpause();
    
    /// <summary>
    /// Pauses the current scene while updating and rendering the provided scene instead.
    /// </summary>
    /// <param name="scene">The scene to update and render instead of the current scene.</param>
    void Pause(IScene scene);
}