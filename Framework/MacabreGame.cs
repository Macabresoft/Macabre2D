namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MacabreGame : Game, IGame {
        protected readonly GraphicsDeviceManager _graphics;
        protected bool _isLoaded;
        protected SpriteBatch _spriteBatch;
        private static IGame _instance = new EmptyGame();
        private IAssetManager _assetManager = new AssetManager();
        private IScene _currentScene;
        private FrameTime _frameTime;
        private double _gameSpeed = 1d;
        private GraphicsSettings _graphicsSettings = new GraphicsSettings();
        private bool _isInitialized;
        private IGameSettings _settings;

        /// <summary>
        /// Initializes the <see cref="MacabreGame"/> class.
        /// </summary>
        static MacabreGame() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MacabreGame"/> class.
        /// </summary>
        public MacabreGame() : base() {
            this._graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.Settings = new GameSettings();
            MacabreGame.Instance = this;
        }

        /// <inheritdoc/>
        public event EventHandler<double> GameSpeedChanged;

        /// <summary>
        /// Gets the singleton instance of <see cref="IGame"/> for the current session.
        /// </summary>
        /// <value>The instance.</value>
        public static IGame Instance {
            get {
                return MacabreGame._instance;
            }

            set {
                if (value != null) {
                    MacabreGame._instance = value;
                }
            }
        }

        /// <inheritdoc/>
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
        [Obsolete("This is a function of MonoGame not used by Macabre2D.", true)]
        public new GameComponentCollection Components {
            get {
                return base.Components;
            }
        }

        /// <inheritdoc/>
        public IScene CurrentScene {
            get {
                return this._currentScene;
            }

            set {
                if (value == null) {
                    throw new NotSupportedException($"{nameof(this.CurrentScene)} cannot be null!");
                }

                if (this._currentScene != value) {
                    this._currentScene = value;

                    if (this._isInitialized) {
                        this._currentScene.Initialize();
                    }

                    if (this._isLoaded) {
                        this._currentScene.LoadContent();
                    }
                }
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public GraphicsSettings GraphicsSettings {
            get {
                return this._graphicsSettings;
            }

            set {
                this._graphicsSettings = value;
                this.ApplyGraphicsSettings();
            }
        }

        /// <inheritdoc/>
        public bool InstantiatePrefabs {
            get {
                return true;
            }
        }

        /// <inheritdoc/>
        public ISaveDataManager SaveDataManager { get; } = new WindowsSaveDataManager(); // TODO: allow other platforms

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public SpriteBatch SpriteBatch {
            get {
                return this._spriteBatch;
            }
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

        /// <inheritdoc/>
        protected override void Draw(GameTime gameTime) {
            if (this.CurrentScene != null) {
                this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
                this.CurrentScene.Draw(this._frameTime);
            }
            else {
                this.GraphicsDevice.Clear(this.Settings.FallbackBackgroundColor);
            }
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            base.Initialize();
            this.CurrentScene?.Initialize();

            if (this.SaveDataManager.TryLoad<GraphicsSettings>(GraphicsSettings.SettingsFileName, out var graphicsSettings)) {
                this.GraphicsSettings = graphicsSettings;
            }
            else {
                this.GraphicsSettings = this.Settings.DefaultGraphicsSettings.Clone();
            }

            this._isInitialized = true;
        }

        /// <inheritdoc/>
        protected override void LoadContent() {
            base.LoadContent();
            this.AssetManager = this.Content.Load<AssetManager>(Framework.AssetManager.ContentFileName);
            this.AssetManager.Initialize(this.Content);
            this.Settings = this.AssetManager.Load<GameSettings>(GameSettings.ContentFileName);
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.CurrentScene = this.AssetManager.Load<Scene>(this.Settings.StartupSceneAssetId);
            this.CurrentScene?.LoadContent();
            this._isLoaded = true;
        }

        /// <inheritdoc/>
        protected override void UnloadContent() {
            base.UnloadContent();
            this.Content.Unload();
        }

        /// <inheritdoc/>
        protected override void Update(GameTime gameTime) {
            var keyboardState = Keyboard.GetState();
            if ((keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt)) && keyboardState.IsKeyDown(Keys.F4)) {
                this.Exit();
            }

            this._frameTime = new FrameTime(gameTime, this.GameSpeed);
            this.CurrentScene?.Update(this._frameTime);
        }
    }
}