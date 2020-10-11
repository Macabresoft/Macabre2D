namespace Macabresoft.MonoGame.AvaloniaUI {

    using Avalonia;
    using Avalonia.Controls;
    using Macabresoft.MonoGame.Core2D;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A view model that essentially acts as a pipeline from Avalonia to a <see cref="IGame" />
    /// </summary>
    public interface IMonoGameViewModel : IDisposable, INotifyPropertyChanged {

        /// <summary>
        /// Gets the game.
        /// </summary>
        /// <value>The game.</value>
        IGame Game { get; }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        GraphicsDevice GraphicsDevice { get; }

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
        /// Runs a single frame.
        /// </summary>
        void RunFrame();
    }

    /// <summary>
    /// A MonoGame view model that acts as a go-between of Avalonia and a MonoGame <see cref="IGame" />.
    /// </summary>
    public abstract class MonoGameViewModel : NotifyPropertyChanged, IMonoGameViewModel {
        private readonly AvaloniaGame _game = new AvaloniaGame();

        private readonly PresentationParameters _presentationParameters = new PresentationParameters() {
            BackBufferWidth = 1,
            BackBufferHeight = 1,
            BackBufferFormat = SurfaceFormat.Color,
            DepthStencilFormat = DepthFormat.Depth24,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
        };

        private bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameViewModel" /> class.
        /// </summary>
        public MonoGameViewModel() {
        }

        /// <inheritdoc />
        public IGame Game => this._game;

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        public GraphicsDevice GraphicsDevice => this._game.GraphicsDevice;

        /// <inheritdoc />
        public void Dispose() {
            if (!this._isDisposed) {
                this._game.Dispose();
                this._isDisposed = true;
            }
        }

        /// <inheritdoc />
        public void Exit() {
            return;
        }

        /// <summary>
        /// Initializes the <see cref="GraphicsDevice" /> with the current <see cref="Window" />.
        /// </summary>
        /// <param name="window">The window.</param>
        public virtual void Initialize(Window window, Size viewportSize, MonoGameMouse mouse, MonoGameKeyboard keyboard) {
            this._game.Initialize(mouse, keyboard);
            this._game.RunOneFrame();
            this._presentationParameters.DeviceWindowHandle = window.PlatformImpl.Handle.Handle;
            this._presentationParameters.BackBufferWidth = Math.Max((int)viewportSize.Width, 1);
            this._presentationParameters.BackBufferHeight = Math.Max((int)viewportSize.Height, 1);
            this.GraphicsDevice.Reset(this._presentationParameters);
        }

        /// <inheritdoc />
        public virtual void Initialize() {
        }

        /// <inheritdoc
        public virtual void LoadScene(IGameScene scene) {
            this._game.LoadScene(scene);
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
                this.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
                this._presentationParameters.BackBufferWidth = Math.Max(width, 1);
                this._presentationParameters.BackBufferHeight = Math.Max(height, 1);
                this.GraphicsDevice.Reset(this._presentationParameters);
            }
        }

        /// <inheritdoc />
        public void RunFrame() {
            this._game.RunOneFrame();
        }
    }
}