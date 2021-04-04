namespace Macabresoft.Macabre2D.Editor.AvaloniaInterop {
    using Avalonia.Input;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A <see cref="IGame" /> that can run within Avalonia.
    /// </summary>
    public interface IAvaloniaGame : IGame, IDisposable, INotifyPropertyChanged {
        /// <summary>
        /// Gets the type of the cursor.
        /// </summary>
        /// <value>The type of the cursor.</value>
        StandardCursorType CursorType { get; set; }

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
        private StandardCursorType _cursorType = StandardCursorType.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaGame" /> class.
        /// </summary>
        public AvaloniaGame() : this(new AssetManager()) {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaGame" /> class.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        protected AvaloniaGame(IAssetManager assetManager) : base(assetManager) {
            this.IsFixedTimeStep = false;
            BaseGame.IsDesignMode = true;
        }

        /// <inheritdoc />
        public GraphicsDeviceManager GraphicsDeviceManager => this._graphics;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the type of the cursor.
        /// </summary>
        /// <value>The type of the cursor.</value>
        public StandardCursorType CursorType {
            get { return this._cursorType; }

            set {
                if (value != this._cursorType) {
                    this._cursorType = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CursorType)));
                }
            }
        }

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
        protected override void UpdateInputState() {
            if (this.Mouse != null && this.Keyboard != null) {
                this.InputState = new InputState(this.Mouse.State, this.Keyboard.GetState(), this.InputState);
            }
        }
    }
}