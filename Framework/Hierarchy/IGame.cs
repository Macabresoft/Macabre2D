namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for the base object running the main loop of a game.
/// </summary>
public interface IGame {
    /// <summary>
    /// Occurs when the culture has changed.
    /// </summary>
    event EventHandler<ResourceCulture> CultureChanged;

    /// <summary>
    /// Occurs when the game speed has changed.
    /// </summary>
    event EventHandler<double> GameSpeedChanged;

    /// <summary>
    /// Occurs when the input device has changed.
    /// </summary>
    event EventHandler<InputDevice> InputDeviceChanged;

    /// <summary>
    /// Occurs when the settings are saved.
    /// </summary>
    event EventHandler SettingsSaved;

    /// <summary>
    /// Occurs when the viewport changes.
    /// </summary>
    event EventHandler<Point> ViewportSizeChanged;

    /// <summary>
    /// Gets the audio settings.
    /// </summary>
    AudioSettings AudioSettings { get; }

    /// <summary>
    /// Gets the content manager.
    /// </summary>
    ContentManager? Content { get; }

    /// <summary>
    /// Gets the scene.
    /// </summary>
    IScene CurrentScene { get; }

    /// <summary>
    /// Gets the data manager.
    /// </summary>
    IDataManager DataManager { get; }

    /// <summary>
    /// Gets the desired input device.
    /// </summary>
    InputDevice DesiredInputDevice { get; }

    /// <summary>
    /// Gets the graphics settings.
    /// </summary>
    DisplaySettings DisplaySettings { get; }

    /// <summary>
    /// Gets the graphics device.
    /// </summary>
    GraphicsDevice? GraphicsDevice { get; }

    /// <summary>
    /// Gets the input bindings.
    /// </summary>
    InputBindings InputBindings { get; }

    /// <summary>
    /// Gets the state of input.
    /// </summary>
    InputState InputState => new();

    /// <summary>
    /// Gets the overlay.
    /// </summary>
    IScene Overlay { get; }

    /// <summary>
    /// Gets the pixel render size when applying shaders.
    /// </summary>
    Point PixelRenderSize { get; }

    /// <summary>
    /// Gets the project.
    /// </summary>
    IGameProject Project { get; }

    /// <summary>
    /// Gets the save manager.
    /// </summary>
    SaveManager SaveManager { get; }

    /// <summary>
    /// Gets the sprite batch.
    /// </summary>
    SpriteBatch? SpriteBatch { get; }

    /// <summary>
    /// Gets the game state.
    /// </summary>
    GameState State { get; }

    /// <summary>
    /// Gets the user settings.
    /// </summary>
    UserSettings UserSettings { get; }

    /// <summary>
    /// Gets the size of the viewport.
    /// </summary>
    Point ViewportSize { get; }

    /// <summary>
    /// Gets or sets the game speed.
    /// </summary>
    double GameSpeed { get; set; }

    /// <summary>
    /// Applies the display settings.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The provided display mode is unsupported.</exception>
    void ApplyDisplaySettings();

    /// <summary>
    /// Exits the game.
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
    /// Pushes a scene onto the scene stack. This becomes the current scene, but other scenes in the stack will still render underneath. Only the current scene will update.
    /// </summary>
    /// <param name="scene">The scene to push.</param>
    void PushScene(IScene scene);

    /// <summary>
    /// Raises the <see cref="CultureChanged" /> event after making sure <see cref="Resources" /> is aware of the change.
    /// </summary>
    /// <remarks>
    /// If you have a settings menu where a user can change culture/language, make sure to call this any time it is changed.
    /// </remarks>
    void RaiseCultureChanged();

    /// <summary>
    /// Saves the applies graphics settings.
    /// </summary>
    void SaveAndApplyUserSettings();

    /// <summary>
    /// Saves the current graphics settings.
    /// </summary>
    void SaveUserSettings();

    /// <summary>
    /// Pops the top scene in the stack. The scene will no longer be updated or rendered. The next scene in the stack will become CurrentScene.
    /// </summary>
    /// <param name="scene">The scene popped.</param>
    /// <returns>A value indicating whether or not a scene was popped.</returns>
    bool TryPopScene(out IScene scene);
}