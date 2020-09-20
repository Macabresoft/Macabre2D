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

        void Draw(GameTime gameTime);

        void Initialize();

        void LoadContent();

        void OnActivated(object sender, EventArgs args);

        void OnDeactivated(object sender, EventArgs args);

        void OnExiting(object sender, EventArgs args);

        void SizeChanged(Size newSize);

        void UnloadContent();

        void Update(GameTime gameTime);
    }

    public abstract class MonoGameViewModel : NotifyPropertyChanged, IMonoGameViewModel {
        private bool _isDisposed = false;

        public MonoGameViewModel() {
            DefaultGame.Instance = this;
        }

        public event EventHandler<double> GameSpeedChanged;

        public event EventHandler<Microsoft.Xna.Framework.Point> ViewportSizeChanged;

        public IAssetManager AssetManager { get; } = new AssetManager();
        public ContentManager Content { get; set; }
        public double GameSpeed { get => 1d; set { return; } }

        public GraphicsDevice GraphicsDevice {
            get {
                return this.GraphicsDeviceService?.GraphicsDevice;
            }
        }

        public MonoGameGraphicsDeviceService GraphicsDeviceService { get; set; }

        public GraphicsSettings GraphicsSettings { get; } = new GraphicsSettings();

        public bool IsDesignMode => true;

        public ISaveDataManager SaveDataManager => Core.SaveDataManager.Empty;

        public IGameScene Scene { get; } = new GameScene();

        public IGameSettings Settings { get; } = new GameSettings();

        public SpriteBatch SpriteBatch { get; private set; }

        public Microsoft.Xna.Framework.Point ViewportSize { get; private set; }

        protected FrameTime FrameTime { get; private set; }

        protected bool IsInitialized { get; private set; }

        protected MonoGameServiceProvider Services { get; private set; }

        public void Dispose() {
            this.Dispose(true);
        }

        public void Draw(GameTime gameTime) {
            if (this.IsInitialized && this.Scene != null) {
                this.Scene.Render(this.FrameTime, new InputState());
            }
        }

        public void Exit() {
            return;
        }

        public virtual void Initialize() {
            this.Services = new MonoGameServiceProvider();
            this.Services.AddService(this.GraphicsDeviceService);
            this.Content = new ContentManager(this.Services) { RootDirectory = "Content" };
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Scene.Initialize(this);
            this.IsInitialized = true;
        }

        public virtual void LoadContent() {
        }

        public virtual void LoadScene(string sceneName) {
            return;
        }

        public virtual void LoadScene(IGameScene scene) {
            return;
        }

        public virtual void OnActivated(object sender, EventArgs args) {
        }

        public virtual void OnDeactivated(object sender, EventArgs args) {
        }

        public virtual void OnExiting(object sender, EventArgs args) {
        }

        public void SaveAndApplyGraphicsSettings() {
            return;
        }

        public void SizeChanged(Avalonia.Size newSize) {
            var originalSize = this.ViewportSize;
            this.ViewportSize = new Microsoft.Xna.Framework.Point(Convert.ToInt32(newSize.Width), Convert.ToInt32(newSize.Height));
            this.ViewportSizeChanged.SafeInvoke(this, this.ViewportSize);
            this.OnViewportChanged(originalSize, this.ViewportSize);
        }

        public virtual void UnloadContent() {
        }

        public virtual void Update(GameTime gameTime) {
            this.FrameTime = new FrameTime(gameTime, this.GameSpeed);
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._isDisposed) {
                if (disposing) {
                    this.Content?.Dispose();
                }

                this._isDisposed = true;
            }
        }

        protected virtual void OnViewportChanged(Microsoft.Xna.Framework.Point originalSize, Microsoft.Xna.Framework.Point newSize) {
        }
    }
}