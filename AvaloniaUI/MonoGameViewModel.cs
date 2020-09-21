namespace Macabresoft.MonoGame.AvaloniaUI {

    using Avalonia;
    using Macabresoft.Core;
    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A view model that essentially acts as a pipeline from Avalonia to a <see cref="IGame" />
    /// </summary>
    public interface IMonoGameViewModel : IGame, IDisposable, INotifyPropertyChanged {
        MonoGameGraphicsDeviceService GraphicsDeviceService { get; set; }

        /// <summary>
        /// Draws the current scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Draw(GameTime gameTime);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Loads the content.
        /// </summary>
        void LoadContent();

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
        /// Unloads the content.
        /// </summary>
        void UnloadContent();

        /// <summary>
        /// Updates the current scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update(GameTime gameTime);
    }

    /// <summary>
    /// A MonoGame view model that acts as a go-between of Avalonia and a MonoGame <see cref="IGame" />.
    /// </summary>
    public abstract class MonoGameViewModel : NotifyPropertyChanged, IMonoGameViewModel {
        private bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameViewModel" /> class.
        /// </summary>
        public MonoGameViewModel() {
            DefaultGame.Instance = this;
        }

        /// <inheritdoc />
        public event EventHandler<double> GameSpeedChanged;

        /// <inheritdoc />
        public event EventHandler<Microsoft.Xna.Framework.Point> ViewportSizeChanged;

        /// <inheritdoc />
        public IAssetManager AssetManager { get; } = new AssetManager();

        /// <inheritdoc />
        public ContentManager Content { get; set; }

        /// <inheritdoc />
        public double GameSpeed { get => 1d; set { return; } }

        /// <inheritdoc />
        public GraphicsDevice GraphicsDevice {
            get {
                return this.GraphicsDeviceService?.GraphicsDevice;
            }
        }

        /// <inheritdoc />
        public MonoGameGraphicsDeviceService GraphicsDeviceService { get; set; }

        /// <inheritdoc />
        public GraphicsSettings GraphicsSettings { get; } = new GraphicsSettings();

        /// <inheritdoc />
        public bool IsDesignMode => true;

        /// <inheritdoc />
        public ISaveDataManager SaveDataManager => Core.SaveDataManager.Empty;

        /// <inheritdoc />
        public IGameScene Scene { get; } = new GameScene();

        /// <inheritdoc
        public IGameSettings Settings { get; } = new GameSettings();

        /// <inheritdoc />
        public SpriteBatch SpriteBatch { get; private set; }

        /// <inheritdoc />
        public Microsoft.Xna.Framework.Point ViewportSize { get; private set; }

        /// <inheritdoc />
        protected FrameTime FrameTime { get; private set; }

        /// <inheritdoc />
        protected bool IsInitialized { get; private set; }

        /// <inheritdoc />
        protected MonoGameServiceProvider Services { get; private set; }

        /// <inheritdoc
        public void Dispose() {
            this.Dispose(true);
        }

        /// <inheritdoc />
        public void Draw(GameTime gameTime) {
            if (this.IsInitialized && this.Scene != null) {
                this.Scene.Render(this.FrameTime, new InputState());
            }
        }

        /// <inheritdoc />
        public void Exit() {
            return;
        }

        /// <inheritdoc />
        public virtual void Initialize() {
            this.Services = new MonoGameServiceProvider();
            this.Services.AddService<IGraphicsDeviceService>(this.GraphicsDeviceService);
            this.Content = new ContentManager(this.Services) { RootDirectory = "Content" };
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.AssetManager.Initialize(this.Content);
            this.Scene.AddSystem<RenderSystem>();
            this.Scene.Initialize(this);
            this.IsInitialized = true;
        }

        /// <inheritdoc
        public virtual void LoadContent() {
        }

        /// <inheritdoc />
        public virtual void LoadScene(string sceneName) {
            return;
        }

        /// <inheritdoc
        public virtual void LoadScene(IGameScene scene) {
            return;
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

        public void OnSizeChanged(Size newSize) {
            this.GraphicsDeviceService.ResetDevice((int)newSize.Width, (int)newSize.Height);

            var originalSize = this.ViewportSize;
            this.ViewportSize = new Microsoft.Xna.Framework.Point(Convert.ToInt32(newSize.Width), Convert.ToInt32(newSize.Height));
            this.ViewportSizeChanged.SafeInvoke(this, this.ViewportSize);
            this.OnViewportChanged(originalSize, this.ViewportSize);
        }

        /// <inheritdoc />
        public void SaveAndApplyGraphicsSettings() {
            return;
        }

        /// <inheritdoc />
        public virtual void UnloadContent() {
        }

        /// <inheritdoc />
        public virtual void Update(GameTime gameTime) {
            this.FrameTime = new FrameTime(gameTime, this.GameSpeed);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) {
            if (!this._isDisposed) {
                if (disposing) {
                    this.Content?.Dispose();
                }

                this._isDisposed = true;
            }
        }

        /// <summary>
        /// Called when the viewport changes.
        /// </summary>
        /// <param name="originalSize">Size of the original viewport.</param>
        /// <param name="newSize">The new vie port size.</param>
        protected virtual void OnViewportChanged(Microsoft.Xna.Framework.Point originalSize, Microsoft.Xna.Framework.Point newSize) {
        }
    }
}