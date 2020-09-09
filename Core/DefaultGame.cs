namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.ComponentModel;

    public class DefaultGame : Game, IGame {

        /// <summary>
        /// The default empty <see cref="IGame" /> that is present before initialization.
        /// </summary>
        public static readonly IGame Empty = new EmptyGame();

        protected readonly GraphicsDeviceManager _graphics;
        protected SpriteBatch? _spriteBatch;
        private static IGame _instance = DefaultGame.Empty;
        private IAssetManager _assetManager = new AssetManager();
        private InputState _currentInputState;
        private FrameTime _frameTime;
        private double _gameSpeed = 1d;
        private GraphicsSettings _graphicsSettings = new GraphicsSettings();
        private bool _isInitialized;
        private IGameScene _scene = GameScene.Empty;
        private IGameSettings _settings = new GameSettings();
        private Point _viewportSize;

        /// <summary>
        /// Initializes the <see cref="DefaultGame" /> class.
        /// </summary>
        static DefaultGame() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultGame" /> class.
        /// </summary>
        public DefaultGame() : base() {
            this._graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.Settings = new GameSettings();
            DefaultGame.Instance = this;
        }

        /// <inheritdoc />
        public event EventHandler<double>? GameSpeedChanged;

        /// <inheritdoc />
        public event EventHandler<Point>? ViewportSizeChanged;

        /// <summary>
        /// Gets the singleton instance of <see cref="IGame" /> for the current session.
        /// </summary>
        /// <value>The instance.</value>
        public static IGame Instance {
            get {
                return DefaultGame._instance;
            }

            set {
                if (value != null) {
                    DefaultGame._instance = value;
                }
            }
        }

        /// <inheritdoc />
        public IAssetManager AssetManager {
            get {
                return this._assetManager;
            }

            set {
                if (value != null) {
                    this._assetManager = value;

                    if (this.Content != null) {
                        this._assetManager.Initialize(this.Content);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <value>The components.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This is a function of MonoGame not used by MacabreGame.", true)]
        public new GameComponentCollection Components {
            get {
                return base.Components;
            }
        }

        /// <inheritdoc />
        public double GameSpeed {
            get {
                return this._gameSpeed;
            }

            set {
                if (value >= 0f && this._gameSpeed != value) {
                    this._gameSpeed = value;
                    this.GameSpeedChanged.SafeInvoke(this, this._gameSpeed);
                }
            }
        }

        /// <inheritdoc />
        public GraphicsSettings GraphicsSettings {
            get {
                return this._graphicsSettings;
            }

            set {
                this._graphicsSettings = value;
                this.ApplyGraphicsSettings();
            }
        }

        /// <inheritdoc />
        public bool IsDesignMode {
            get {
                return false;
            }
        }

        /// <inheritdoc />
        public ISaveDataManager SaveDataManager { get; } = new WindowsSaveDataManager();

        /// <inheritdoc />
        public IGameScene Scene {
            get {
                return this._scene;
            }

            private set {
                if (value == null) {
                    throw new NotSupportedException($"{nameof(this.Scene)} cannot be null!");
                }

                if (this._scene != value) {
                    this._scene = value;

                    if (this._isInitialized) {
                        this._scene.Initialize(this);
                    }
                }
            }
        }

        /// <inheritdoc />
        public IGameSettings Settings {
            get {
                return this._settings;
            }

            set {
                if (value != null) {
                    this._settings = value;
                    GameSettings.Instance = this._settings;
                }
            }
        }

        /// <inheritdoc />
        public SpriteBatch? SpriteBatch {
            get {
                return this._spriteBatch;
            }
        }

        /// <inheritdoc />
        public Point ViewportSize {
            get {
                return this._viewportSize;
            }
        }

        /// <inheritdoc />
        public void LoadScene(string sceneName) {
            if (this.AssetManager.TryLoad<GameScene>(sceneName, out var scene) && scene != null) {
                this.LoadScene(scene);
            }
            else {
                throw new ArgumentException($"'{sceneName}' could not be found.");
            }
        }

        /// <inheritdoc />
        public void LoadScene(IGameScene scene) {
            this.Scene = scene;
        }

        public void SaveAndApplyGraphicsSettings() {
            this.SaveDataManager.Save(GraphicsSettings.SettingsFileName, this.GraphicsSettings);
            this.ApplyGraphicsSettings();
        }

        public void SaveGraphicsSettings() {
            this.SaveDataManager.Save(GraphicsSettings.SettingsFileName, this.GraphicsSettings);
        }

        protected virtual void ApplyGraphicsSettings() {
            if (this.GraphicsSettings.DisplayMode == DisplayModes.Borderless) {
                this._graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                this._graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                this.Window.IsBorderless = true;
                this._graphics.IsFullScreen = false;
            }
            else {
                this._graphics.PreferredBackBufferWidth = this.GraphicsSettings.Resolution.X;
                this._graphics.PreferredBackBufferHeight = this.GraphicsSettings.Resolution.Y;

                if (this.GraphicsSettings.DisplayMode == DisplayModes.Fullscreen) {
                    this.Window.IsBorderless = false;
                    this._graphics.IsFullScreen = true;
                }
                else if (this.GraphicsSettings.DisplayMode == DisplayModes.Windowed) {
                    this.Window.IsBorderless = false;
                    this._graphics.IsFullScreen = false;
                }
            }

            this._graphics.ApplyChanges();
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime) {
            this.Scene.Render(this._frameTime);
        }

        /// <inheritdoc />
        protected override void Initialize() {
            base.Initialize();

            this._viewportSize = new Point(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
            this.Scene?.Initialize(this);

            if (this.SaveDataManager.TryLoad<GraphicsSettings>(GraphicsSettings.SettingsFileName, out var graphicsSettings) && graphicsSettings != null) {
                this.GraphicsSettings = graphicsSettings;
            }
            else {
                this.GraphicsSettings = this.Settings.DefaultGraphicsSettings.Clone();
            }

            this._isInitialized = true;
        }

        /// <inheritdoc />
        protected override void LoadContent() {
            base.LoadContent();
            this.AssetManager = this.Content.Load<AssetManager>(Core.AssetManager.ContentFileName);
            this.AssetManager.Initialize(this.Content);

            if (this.AssetManager.TryLoad<GameSettings>(GameSettings.ContentFileName, out var gameSettings) && gameSettings != null) {
                this.Settings = gameSettings;
            }

            if (this.AssetManager.TryLoad<GameScene>(this.Settings.StartupSceneAssetId, out var scene) && scene != null) {
                this.LoadScene(scene);
            }

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        /// <inheritdoc />
        protected override void UnloadContent() {
            base.UnloadContent();
            this.Content.Unload();
        }

        /// <inheritdoc />
        protected override void Update(GameTime gameTime) {
            var keyboardState = Keyboard.GetState();
            if ((keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt)) && keyboardState.IsKeyDown(Keys.F4)) {
                this.Exit();
            }

            if (this._viewportSize.X != this.GraphicsDevice.Viewport.Width || this._viewportSize.Y != this.GraphicsDevice.Viewport.Height) {
                this._viewportSize = new Point(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
                this.ViewportSizeChanged.SafeInvoke(this, this._viewportSize);
            }

            this._currentInputState = new InputState(Mouse.GetState(), keyboardState, this._currentInputState);
            this._frameTime = new FrameTime(gameTime, this.GameSpeed);
            this.Scene.Update(this._frameTime, this._currentInputState);
        }

        private sealed class EmptyGame : IGame {

            /// <inheritdoc />
            public event EventHandler<double>? GameSpeedChanged;

            /// <inheritdoc />
            public event EventHandler<Point>? ViewportSizeChanged;

            /// <inheritdoc />
            public IAssetManager AssetManager { get; } = new AssetManager();

            public ContentManager? Content => null;

            /// <inheritdoc />
            public double GameSpeed {
                get {
                    return 1f;
                }

                set {
                    this.GameSpeedChanged.SafeInvoke(this, 1f);
                }
            }

            public GraphicsDevice? GraphicsDevice => null;

            /// <inheritdoc />
            public GraphicsSettings GraphicsSettings { get; } = new GraphicsSettings();

            /// <inheritdoc />
            public bool IsDesignMode {
                get {
                    return true;
                }
            }

            /// <inheritdoc />
            public ISaveDataManager SaveDataManager { get; } = new EmptySaveDataManager();

            /// <inheritdoc />
            public IGameScene Scene => GameScene.Empty;

            /// <inheritdoc />
            public IGameSettings Settings {
                get {
                    return GameSettings.Instance;
                }
            }

            /// <inheritdoc />
            public SpriteBatch? SpriteBatch => null;

            /// <inheritdoc />
            public Point ViewportSize {
                get {
                    return default;
                }

                set {
                    this.ViewportSizeChanged.SafeInvoke(this, default);
                }
            }

            public void Exit() {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void LoadScene(string sceneName) {
                return;
            }

            /// <inheritdoc />
            public void LoadScene(IGameScene scene) {
                return;
            }

            /// <inheritdoc />
            public void SaveAndApplyGraphicsSettings() {
                return;
            }
        }
    }
}