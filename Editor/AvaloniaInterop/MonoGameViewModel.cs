namespace Macabresoft.Macabre2D.UI.AvaloniaInterop {
    using System;
    using System.ComponentModel;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using ReactiveUI;

    /// <summary>
    /// A view model that essentially acts as a pipeline from Avalonia to a <see cref="IGame" />
    /// </summary>
    public interface IMonoGameViewModel : IDisposable, INotifyPropertyChanged {
        /// <summary>
        /// Gets or sets the type of the cursor.
        /// </summary>
        /// <value>The type of the cursor.</value>
        public StandardCursorType CursorType { get; }

        /// <summary>
        /// Gets the game.
        /// </summary>
        /// <value>The game.</value>
        IAvaloniaGame Game { get; }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        IScene Scene { get; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="viewportSize">Size of the viewport.</param>
        /// <param name="mouse">The mouse.</param>
        /// <param name="keyboard">The keyboard.</param>
        void Initialize(Window window, Size viewportSize, MonoGameMouse mouse, MonoGameKeyboard keyboard);

        /// <summary>
        /// Called when activated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        void OnActivated(object sender, EventArgs args);

        /// <summary>
        /// Called when deactivated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        void OnDeactivated(object sender, EventArgs args);

        /// <summary>
        /// Called when exiting.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        void OnExiting(object sender, EventArgs args);

        /// <summary>
        /// Called when the control's size changed.
        /// </summary>
        /// <param name="newSize">The new size.</param>
        void OnSizeChanged(Size newSize);

        /// <summary>
        /// Resets the graphics device with a new size.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        void ResetDevice(int width, int height);

        /// <summary>
        /// Resets the scene.
        /// </summary>
        void ResetScene();

        /// <summary>
        /// Runs a single frame.
        /// </summary>
        void RunFrame();
    }

    /// <summary>
    /// A MonoGame view model that acts as a go-between of Avalonia and a MonoGame <see cref="IGame" />.
    /// </summary>
    public abstract class MonoGameViewModel : ReactiveObject, IMonoGameViewModel {
        private readonly PresentationParameters _presentationParameters = new() {
            BackBufferWidth = 1,
            BackBufferHeight = 1,
            BackBufferFormat = SurfaceFormat.Color,
            DepthStencilFormat = DepthFormat.Depth24,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
        };

        private bool _isDisposed;
        private MonoGameKeyboard _keyboard;
        private MonoGameMouse _mouse;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        protected MonoGameViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameViewModel" /> class.
        /// </summary>
        /// <param name="game">The game.</param>
        protected MonoGameViewModel(IAvaloniaGame game) {
            this.Game = game;
            this.Game.PropertyChanged += this.Game_PropertyChanged;
        }

        /// <summary>
        /// Gets or sets the type of the cursor.
        /// </summary>
        /// <value>The type of the cursor.</value>
        public StandardCursorType CursorType => this.Game.CursorType;

        /// <inheritdoc />
        public IAvaloniaGame Game { get; }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        public GraphicsDevice GraphicsDevice => this.Game.GraphicsDevice;

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        public IScene Scene { get; private set; }

        /// <inheritdoc />
        public void Dispose() {
            if (!this._isDisposed) {
                this.Game.PropertyChanged -= this.Game_PropertyChanged;
                this.Game.Dispose();
                this._isDisposed = true;
            }
        }

        /// <summary>
        /// Initializes the <see cref="GraphicsDevice" /> with the current <see cref="Window" />.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="viewportSize"></param>
        /// <param name="mouse"></param>
        /// <param name="keyboard"></param>
        public virtual void Initialize(Window window, Size viewportSize, MonoGameMouse mouse, MonoGameKeyboard keyboard) {
            this._mouse = mouse;
            this._keyboard = keyboard;
            this.Game.Initialize(mouse, keyboard);
            this.Game.RunOneFrame();
            this.Scene = this.Game.Scene;

            var width = Math.Max((int)viewportSize.Width, 1);
            var height = Math.Max((int)viewportSize.Height, 1);
            this._presentationParameters.DeviceWindowHandle = window.PlatformImpl.Handle.Handle;
            this._presentationParameters.BackBufferWidth = width;
            this._presentationParameters.BackBufferHeight = height;
            this.Game.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            this.Game.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            this.GraphicsDevice.Reset(this._presentationParameters);
        }

        /// <inheritdoc
        public virtual void OnActivated(object sender, EventArgs args) {
        }

        /// <inheritdoc />
        public virtual void OnDeactivated(object sender, EventArgs args) {
        }

        /// <inheritdoc />
        public virtual void OnExiting(object sender, EventArgs args) {
        }

        /// <inheritdoc />
        public void OnSizeChanged(Size newSize) {
            this.ResetDevice((int)newSize.Width, (int)newSize.Height);
        }

        /// <inheritdoc />
        public void ResetDevice(int width, int height) {
            var newWidth = Math.Max(1, width);
            var newHeight = Math.Max(1, height);

            if (newWidth != this.GraphicsDevice.PresentationParameters.BackBufferWidth || newHeight != this.GraphicsDevice.PresentationParameters.BackBufferHeight) {
                this.Game.GraphicsDeviceManager.PreferredBackBufferWidth = newWidth;
                this.Game.GraphicsDeviceManager.PreferredBackBufferHeight = newHeight;
                this.GraphicsDevice.Viewport = new Viewport(0, 0, newWidth, newHeight);
                this._presentationParameters.BackBufferWidth = newWidth;
                this._presentationParameters.BackBufferHeight = newHeight;
                this.GraphicsDevice.Reset(this._presentationParameters);
            }
        }

        /// <inheritdoc />
        public void ResetScene() {
            if (this.Scene != null) {
                this.Game.LoadScene(this.Scene);
                this.Game.Initialize(this._mouse, this._keyboard);
            }
        }

        /// <inheritdoc />
        public void RunFrame() {
            try {
                this.Game.RunOneFrame();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private void Game_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IAvaloniaGame.CursorType)) {
                this.RaisePropertyChanged(nameof(this.CursorType));
            }
        }
    }
}