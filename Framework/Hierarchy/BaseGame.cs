namespace Macabresoft.Macabre2D.Framework;

using System;
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

    private double _gameSpeed = 1d;
    private GraphicsSettings _graphicsSettings = new();
    private IGameProject _project = new GameProject();
    private IScene _scene = Framework.Scene.Empty;
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
    public BaseGame() : base() {
        this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
        this.Content.RootDirectory = "Content";
    }

    /// <inheritdoc />
    public double GameSpeed {
        get => this._gameSpeed;

        set {
            if (value >= 0f && Math.Abs(this._gameSpeed - value) > 0.001f) {
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
    public InputBindings InputBindings { get; private set; } = new();

    /// <inheritdoc />
    public InputState InputState { get; protected set; }

    /// <summary>
    /// Gets or sets a value which indicates whether or not the game is running in design mode.
    /// </summary>
    public static bool IsDesignMode { get; set; }

    /// <inheritdoc />
    public IGameProject Project {
        get => this._project;
        protected set {
            this._project = value;
            this.ApplyGraphicsSettings();
        }
    }

    /// <inheritdoc />
    public ISaveDataManager SaveDataManager { get; } = new WindowsSaveDataManager();

    /// <inheritdoc />
    public IScene Scene {
        get => this._scene;

        private set {
            if (this._scene != value) {
                this._scene = value;

                if (this.IsInitialized) {
                    this._scene.Initialize(this, this.CreateAssetManager());
                }
            }
        }
    }

    /// <inheritdoc />
    public SpriteBatch? SpriteBatch { get; private set; }

    /// <inheritdoc />
    public Point ViewportSize => this._viewportSize;

    /// <summary>
    /// Gets the frame time.
    /// </summary>
    /// <value>The frame time.</value>
    protected FrameTime FrameTime { get; private set; }

    /// <summary>
    /// Gets the graphics device manager.
    /// </summary>
    protected GraphicsDeviceManager GraphicsDeviceManager { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is initialized.
    /// </summary>
    /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
    protected bool IsInitialized { get; private set; }

    /// <inheritdoc />
    public void LoadScene(string sceneName) {
        var assetManager = this.CreateAssetManager();
        if (assetManager.TryLoadContent<Scene>(sceneName, out var scene) && scene != null) {
            this.LoadScene(scene);
        }
        else {
            throw new ArgumentException($"'{sceneName}' could not be found.");
        }

        if (!IsDesignMode) {
            assetManager.Dispose();
        }
    }

    /// <inheritdoc />
    public void LoadScene(IScene scene) {
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
    public void SaveInputBindings() {
        this.SaveDataManager.Save(InputBindings.SettingsFileName, this.Project.Name, this.InputBindings);
    }

    /// <summary>
    /// Creates a scene level asset manager.
    /// </summary>
    /// <returns>The asset manager.</returns>
    protected virtual IAssetManager CreateAssetManager() {
        var assetManager = new AssetManager();
        var contentManager = new ContentManager(this.Content.ServiceProvider, this.Content.RootDirectory);
        assetManager.Initialize(contentManager, Serializer.Instance);
        this.RegisterNewSceneMetadata(assetManager);
        return assetManager;
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
        this._scene.Initialize(this, this.CreateAssetManager());

        if (IsDesignMode) {
            this.GraphicsSettings = this.Project.Settings.DefaultGraphicsSettings.Clone();
        }
        else {
            if (this.SaveDataManager.TryLoad<GraphicsSettings>(GraphicsSettings.SettingsFileName, this.Project.Name, out var graphicsSettings) && graphicsSettings != null) {
                this.GraphicsSettings = graphicsSettings;
            }
            else {
                this.GraphicsSettings = this.Project.Settings.DefaultGraphicsSettings.Clone();
            }

            if (this.SaveDataManager.TryLoad<InputBindings>(InputBindings.SettingsFileName, this.Project.Name, out var inputBindings) && inputBindings != null) {
                this.InputBindings = inputBindings;
            }
            else {
                this.InputBindings = this.Project.Settings.InputSettings.DefaultBindings.Clone();
            }
        }


        this.IsInitialized = true;
    }

    /// <inheritdoc />
    protected override void LoadContent() {
        base.LoadContent();
        var assetManager = this.CreateAssetManager();
        if (assetManager.TryLoadContent<GameProject>(GameProject.ProjectFileName, out var project) && project != null) {
            this.Project = project;
        }

        if (assetManager.TryLoadContent<Scene>(this.Project.StartupSceneContentId, out var scene) && scene != null) {
            this.LoadScene(scene);
        }

        this.TryCreateSpriteBatch();

        if (!IsDesignMode) {
            assetManager.Dispose();
        }
    }

    /// <summary>
    /// Called just before a scene is initialized with its new asset manager.
    /// </summary>
    protected virtual void RegisterNewSceneMetadata(IAssetManager assetManager) {
    }

    /// <summary>
    /// Creates the sprite batch if it doesn't already exist.
    /// </summary>
    protected void TryCreateSpriteBatch() {
        this.SpriteBatch ??= new SpriteBatch(this.GraphicsDevice);
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
        this.InputState = new InputState(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One), this.InputState);
    }

    private void ApplyGraphicsSettings() {
        if (this.GraphicsSettings.DisplayMode == DisplayModes.Borderless) {
            this.GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.Window.IsBorderless = true;
            this.GraphicsDeviceManager.IsFullScreen = false;
        }
        else {
            this.GraphicsDeviceManager.PreferredBackBufferWidth = this.GraphicsSettings.Resolution.X;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = this.GraphicsSettings.Resolution.Y;

            switch (this.GraphicsSettings.DisplayMode) {
                case DisplayModes.Fullscreen:
                    this.Window.IsBorderless = false;
                    this.GraphicsDeviceManager.IsFullScreen = true;
                    break;
                case DisplayModes.Windowed:
                    this.Window.IsBorderless = false;
                    this.GraphicsDeviceManager.IsFullScreen = false;
                    break;
                case DisplayModes.Borderless:
                    this.Window.IsBorderless = true;
                    this.GraphicsDeviceManager.IsFullScreen = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        this.GraphicsDeviceManager.ApplyChanges();
    }

    private sealed class EmptyGame : IGame {
        public event EventHandler<double>? GameSpeedChanged;

        public event EventHandler<Point>? ViewportSizeChanged;

        public ContentManager? Content => null;

        public double GameSpeed {
            get => 1f;
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.GameSpeedChanged.SafeInvoke(this, 1f);
            }
        }

        public GraphicsDevice? GraphicsDevice => null;

        public GraphicsSettings GraphicsSettings { get; } = new();

        public InputBindings InputBindings { get; } = new();

        public IGameProject Project { get; } = new GameProject();

        public ISaveDataManager SaveDataManager { get; } = new EmptySaveDataManager();

        public IScene Scene => Framework.Scene.Empty;

        public SpriteBatch? SpriteBatch => null;

        public Point ViewportSize => default;

        public void Exit() {
        }

        public void LoadScene(string sceneName) {
        }

        public void LoadScene(IScene scene) {
        }

        public void SaveAndApplyGraphicsSettings() {
        }

        public void SaveGraphicsSettings() {
        }

        public void SaveInputBindings() {
        }
    }
}