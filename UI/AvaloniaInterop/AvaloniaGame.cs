namespace Macabresoft.Macabre2D.UI.AvaloniaInterop;

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A <see cref="IGame" /> that can run within Avalonia.
/// </summary>
public interface IAvaloniaGame : IGame, IDisposable, INotifyPropertyChanged {
    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    IAssetManager Assets { get; }

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

    /// <summary>
    /// Performs a tick in the game's update loop.
    /// </summary>
    void Tick();
}

/// <summary>
/// A minimal instance of <see cref="Game" /> that is run for Avalonia.
/// </summary>
public class AvaloniaGame : BaseGame, IAvaloniaGame {
    private MonoGameKeyboard _keyboard;
    private MonoGameMouse _mouse;

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
    public void Initialize(MonoGameMouse mouse, MonoGameKeyboard keyboard) {
        this._mouse = mouse;
        this._keyboard = keyboard;
    }

    /// <inheritdoc />
    protected override IAssetManager CreateAssetManager() {
        return this.Assets;
    }

    /// <inheritdoc />
    protected override void LoadContent() {
        this.TryCreateSpriteBatch();
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
        if (this._mouse != null && this._keyboard != null) {
            this.InputState = new InputState(this._mouse.State, this._keyboard.GetState(), GamePadState.Default, this.InputState);
        }
    }
}