namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The base game class for Macabre2D.
    /// </summary>
    public class BaseGame : Game, IGame {
        /// <summary>
        /// The default empty <see cref="IGame" /> that is present before initialization.
        /// </summary>
        public static readonly IGame Empty = new EmptyGame();

        // ReSharper disable once InconsistentNaming
        protected readonly GraphicsDeviceManager _graphics;

        // ReSharper disable once InconsistentNaming
        protected SpriteBatch? _spriteBatch;

        private double _gameSpeed = 1d;
        private GraphicsSettings _graphicsSettings = new();
        private IGameProject _project = new GameProject();
        private IGameScene _scene = GameScene.Empty;
        private Point _viewportSize;

        /// <inheritdoc />
        public event EventHandler<double>? GameSpeedChanged;

        /// <inheritdoc />
        public event EventHandler<Point>? ViewportSizeChanged;

        /// <summary>
        /// Initializes the <see cref="BaseGame" /> class.
        /// </summary>
        static BaseGame() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGame" /> class.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        protected BaseGame(IAssetManager assetManager) : base() {
            this.Assets = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
            this._graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.Assets.Initialize(this.Content, Serializer.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGame" /> class.
        /// </summary>
        protected BaseGame() : this(new AssetManager()) {
        }

        /// <inheritdoc />
        public IAssetManager Assets { get; }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <value>The components.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This is a function of MonoGame not used by MacabreGame.", true)]
        public new GameComponentCollection Components => base.Components;

        /// <inheritdoc />
        public ISaveDataManager SaveDataManager { get; } = new WindowsSaveDataManager();

        /// <inheritdoc />
        public SpriteBatch? SpriteBatch => this._spriteBatch;

        /// <inheritdoc />
        public Point ViewportSize => this._viewportSize;

        /// <inheritdoc />
        public double GameSpeed {
            get => this._gameSpeed;

            set {
                if (value >= 0f && this._gameSpeed != value) {
                    this._gameSpeed = value;
                    this.GameSpeedChanged.SafeInvoke(this, this._gameSpeed);
                }
            }
        }

        /// <inheritdoc />
        public GraphicsSettings GraphicsSettings {
            get => this._graphicsSettings;
            private set {
                this._graphicsSettings = value;
                this.ApplyGraphicsSettings();
            }
        }

        /// <inheritdoc />
        public InputState InputState { get; protected set; }

        /// <summary>
        /// Gets a value which indicates whether or not the game is running in design mode.
        /// </summary>
        public static bool IsDesignMode { get; protected set; }

        /// <inheritdoc />
        public IGameProject Project {
            get => this._project;
            protected set {
                this._project = value;
                this.ApplyGraphicsSettings();
            }
        }

        /// <inheritdoc />
        public IGameScene Scene {
            get => this._scene;

            private set {
                if (this._scene != value) {
                    this._scene = value;

                    if (this.IsInitialized) {
                        this._scene.Initialize(this, this.CreateSceneLevelAssetManager());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the frame time.
        /// </summary>
        /// <value>The frame time.</value>
        protected FrameTime FrameTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        protected bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public void LoadScene(string sceneName) {
            if (this.Assets.TryLoadContent<GameScene>(sceneName, out var scene) && scene != null) {
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

        /// <inheritdoc />
        public void SaveAndApplyGraphicsSettings() {
            this.SaveGraphicsSettings();
            this.ApplyGraphicsSettings();
        }

        /// <inheritdoc />
        public void SaveGraphicsSettings() {
            this.SaveDataManager.Save(GraphicsSettings.SettingsFileName, this.Project.Name, this.GraphicsSettings);
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime) {
            if (this.GraphicsDevice != null) {
                this.GraphicsDevice.Clear(this.Scene.BackgroundColor);

                this.Scene.Render(this.FrameTime, this.InputState);
            }
        }

        /// <inheritdoc />
        protected override void Initialize() {
            base.Initialize();

            this._viewportSize = new Point(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
            this._scene.Initialize(this, this.CreateSceneLevelAssetManager());

            if (this.SaveDataManager.TryLoad<GraphicsSettings>(GraphicsSettings.SettingsFileName, this.Project.Name, out var graphicsSettings) && graphicsSettings != null) {
                this.GraphicsSettings = graphicsSettings;
            }
            else {
                this.GraphicsSettings = this.Project.Settings.DefaultGraphicsSettings.Clone();
            }

            this.IsInitialized = true;
        }

        /// <summary>
        /// Creates a scene level asset manager.
        /// </summary>
        /// <returns>The asset manager.</returns>
        protected virtual IAssetManager CreateSceneLevelAssetManager() {
            var assetManager = new AssetManager();
            var contentManager = new ContentManager(this.Content.ServiceProvider, this.Content.RootDirectory);
            assetManager.Initialize(contentManager, Serializer.Instance);
            return assetManager;
        }

        /// <inheritdoc />
        protected override void LoadContent() {
            base.LoadContent();

            if (this.Assets.TryLoadContent<GameProject>(GameProject.ProjectFileName, out var project) && project != null) {
                this.Project = project;
            }

            if (this.Assets.TryLoadContent<GameScene>(this.Project.StartupSceneContentId, out var scene) && scene != null) {
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
            if (!IsDesignMode) {
                var keyboardState = Keyboard.GetState();
                if ((keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt)) && keyboardState.IsKeyDown(Keys.F4)) {
                    this.Exit();
                }
            }

            if (this._viewportSize.X != this.GraphicsDevice.Viewport.Width || this._viewportSize.Y != this.GraphicsDevice.Viewport.Height) {
                this._viewportSize = new Point(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
                this.ViewportSizeChanged.SafeInvoke(this, this._viewportSize);
            }

            this.UpdateInputState();
            this.FrameTime = new FrameTime(gameTime, this.GameSpeed);
            this.Scene.Update(this.FrameTime, this.InputState);
        }

        /// <summary>
        /// Updates the <see cref="InputState" />.
        /// </summary>
        protected virtual void UpdateInputState() {
            this.InputState = new InputState(Mouse.GetState(), Keyboard.GetState(), this.InputState);
        }

        private void ApplyGraphicsSettings() {
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
        
        private sealed class EmptyGame : IGame {
            /// <inheritdoc />
            public event EventHandler<double>? GameSpeedChanged;

            /// <inheritdoc />
            public event EventHandler<Point>? ViewportSizeChanged;

            /// <inheritdoc />
            public IAssetManager Assets => AssetManager.Empty;

            /// <inheritdoc />
            public ContentManager? Content => null;

            /// <inheritdoc />
            public GraphicsDevice? GraphicsDevice => null;

            /// <inheritdoc />
            public GraphicsSettings GraphicsSettings { get; } = new();

            /// <inheritdoc />
            public IGameProject Project { get; } = new GameProject();

            /// <inheritdoc />
            public ISaveDataManager SaveDataManager { get; } = new EmptySaveDataManager();

            /// <inheritdoc />
            public IGameScene Scene => GameScene.Empty;

            /// <inheritdoc />
            public SpriteBatch? SpriteBatch => null;

            /// <inheritdoc />
            public Point ViewportSize => default;

            /// <inheritdoc />
            public double GameSpeed {
                get => 1f;
                set {
                    if (value <= 0) {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }

                    this.GameSpeedChanged.SafeInvoke(this, 1f);
                }
            }

            /// <inheritdoc />
            public void Exit() {
            }

            /// <inheritdoc />
            public void LoadScene(string sceneName) {
            }

            /// <inheritdoc />
            public void LoadScene(IGameScene scene) {
            }

            /// <inheritdoc />
            public void SaveAndApplyGraphicsSettings() {
            }

            /// <inheritdoc />
            public void SaveGraphicsSettings() {
            }
        }
    }
}