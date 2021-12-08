namespace Macabresoft.Macabre2D.UI.AvaloniaInterop;

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A <see cref="IGame" /> that can run within Avalonia.
/// </summary>
public interface IAvaloniaGame : IGame, IDisposable, INotifyPropertyChanged {
    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    IAssetManager Assets { get; }

    /// <summary>
    /// Gets the graphics device manager for this game.
    /// </summary>
    public GraphicsDeviceManager GraphicsDeviceManager { get; }

    /// <summary>
    /// Initializes the specified mouse.
    /// </summary>
    /// <param name="mouse">The mouse.</param>
    /// <param name="keyboard">The keyboard.</param>
    void Initialize(MonoGameMouse mouse, MonoGameKeyboard keyboard);

    /// <summary>
    /// Runs one frame.
    /// </summary>
    void RunOneFrame();
}

/// <summary>
/// A minimal instance of <see cref="Game" /> that is run for Avalonia.
/// </summary>
public class AvaloniaGame : BaseGame, IAvaloniaGame {
    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaGame" /> class.
    /// </summary>
    public AvaloniaGame() : this(new AssetManager()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaGame" /> class.
    /// </summary>
    /// <param name="assetManager">The asset manager.</param>
    protected AvaloniaGame(IAssetManager assetManager) : base() {
        this.Assets = assetManager;
        this.IsFixedTimeStep = false;
        IsDesignMode = true;
    }

    /// <inheritdoc />
    public IAssetManager Assets { get; }

    /// <inheritdoc />
    public GraphicsDeviceManager GraphicsDeviceManager => this._graphics;

    /// <summary>
    /// Gets the keyboard.
    /// </summary>
    /// <value>The keyboard.</value>
    public MonoGameKeyboard Keyboard { get; private set; }

    /// <summary>
    /// Gets the mouse.
    /// </summary>
    /// <value>The mouse.</value>
    public MonoGameMouse Mouse { get; private set; }

    /// <inheritdoc />
    public void Initialize(MonoGameMouse mouse, MonoGameKeyboard keyboard) {
        this.Mouse = mouse;
        this.Keyboard = keyboard;
    }

    /// <inheritdoc />
    protected override IAssetManager CreateAssetManager() {
        return this.Assets;
    }

    /// <inheritdoc />
    protected override void LoadContent() {
        this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        this.Assets.Initialize(this.Content, new Serializer());
    }

    /// <summary>
    /// Raises a property changed notification.
    /// </summary>
    /// <param name="propertyName">The property name</param>
    protected void RaisePropertyChanged([CallerMemberName] string propertyName = "") {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc />
    protected override void UpdateInputState() {
        if (this.Mouse != null && this.Keyboard != null) {
            this.InputState = new InputState(this.Mouse.State, this.Keyboard.GetState(), this.InputState);
        }
    }
}