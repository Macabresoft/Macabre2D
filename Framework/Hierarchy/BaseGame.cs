namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Project.Common;
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

    private readonly List<GameTransition> _runningTransitions = new();
    private readonly Stack<IScene> _sceneStack = new();
    private readonly Dictionary<Guid, RenderTarget2D> _screenShaderIdToRenderTargets = new();
    private bool _canToggleFullscreen = true;
    private InputDevice _desiredInputDevice = InputDevice.Auto;
    private RenderTarget2D? _gameRenderTarget;
    private double _gameSpeed = 1d;
    private IGameProject _project = new GameProject();
    private UserSettings _userSettings = new();

    /// <inheritdoc />
    public event EventHandler<ResourceCulture>? CultureChanged;

    /// <inheritdoc />
    public event EventHandler<double>? GameSpeedChanged;

    /// <inheritdoc />
    public event EventHandler<InputDevice>? InputDeviceChanged;

    /// <inheritdoc />
    public event EventHandler? SettingsSaved;

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
    public BaseGame(string[] arguments) : this(arguments.ToLaunchArguments()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseGame" /> class.
    /// </summary>
    public BaseGame() : base() {
        this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
        this.Content.RootDirectory = "Content";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseGame" /> class.
    /// </summary>
    protected BaseGame(LaunchArguments launchArguments) : this() {
        this.LaunchArguments = launchArguments;
        IsDesignMode = launchArguments.HasFlag(LaunchArguments.EditorMode);
    }

    /// <inheritdoc />
    public AudioSettings AudioSettings => this.UserSettings.Audio;

    /// <inheritdoc />
    public IScene CurrentScene {
        get => this._sceneStack.Any() ? this._sceneStack.Peek() : EmptyObject.Scene;
        private set {
            if (this.CurrentScene != value) {
                this.ClearSceneStack();
                this._sceneStack.Push(value);

                if (this.IsInitialized) {
                    value.Initialize(this, this.CreateAssetManager());
                }
            }
        }
    }

    /// <inheritdoc />
    public IDataManager DataManager { get; } = new WindowsDataManager();

    /// <inheritdoc />
    public InputDevice DesiredInputDevice {
        get => this._desiredInputDevice;
        protected set {
            if (value != this._desiredInputDevice && value != InputDevice.Auto) {
                this._desiredInputDevice = value;
                this.InputDeviceChanged.SafeInvoke(this, this._desiredInputDevice);
            }
        }
    }

    /// <inheritdoc />
    public DisplaySettings DisplaySettings => this.UserSettings.Display;

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
    public InputBindings InputBindings => this.UserSettings.Input;

    /// <inheritdoc />
    public InputState InputState { get; protected set; }

    /// <summary>
    /// Gets or sets a value which indicates whether the game is running in design mode.
    /// </summary>
    public static bool IsDesignMode { get; private set; }

    /// <inheritdoc />
    public LaunchArguments LaunchArguments { get; }

    /// <inheritdoc />
    public IScene Overlay { get; private set; } = EmptyObject.Scene;

    /// <inheritdoc />
    public Point PixelRenderSize { get; private set; }

    /// <inheritdoc />
    public IGameProject Project {
        get => this._project;
        protected set {
            this._project = value;
            this.ApplyDisplaySettings();
        }
    }

    /// <inheritdoc />
    public SpriteBatch? SpriteBatch { get; private set; }

    /// <inheritdoc />
    public GameState State { get; } = new();

    /// <inheritdoc />
    public UserSettings UserSettings {
        get => this._userSettings;
        private set {
            this._userSettings = value;
            this.ApplyDisplaySettings();
        }
    }

    /// <inheritdoc />
    public Point ViewportSize { get; private set; }

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
    protected bool IsInitialized { get; private set; }

    /// <inheitdoc />
    public void ApplyDisplaySettings() {
        if (this.DisplaySettings.DisplayMode == DisplayMode.Borderless) {
            this.GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.Window.IsBorderless = true;
            this.GraphicsDeviceManager.IsFullScreen = false;
        }
        else {
            var verticalPixels = (int)Math.Ceiling(this.Project.CommonViewHeight * this.Project.PixelsPerUnit);
            var resolution = this.DisplaySettings.GetResolution(verticalPixels);
            this.GraphicsDeviceManager.PreferredBackBufferWidth = resolution.X;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = resolution.Y;

            if (this.DisplaySettings.DisplayMode == DisplayMode.Windowed) {
                this.Window.IsBorderless = false;
                this.GraphicsDeviceManager.IsFullScreen = false;
            }
            else {
                this.Window.IsBorderless = false;
                this.GraphicsDeviceManager.IsFullScreen = true;
            }
        }

        this.GraphicsDeviceManager.ApplyChanges();
        this.ResetViewPort();
    }

    /// <inheritdoc />
    public void BeginTransition(GameTransition transition) {
        this._runningTransitions.Add(transition);
    }

    /// <inheritdoc />
    public void LoadScene(string sceneName) {
        var assetManager = this.CreateAssetManager();
        if (assetManager.TryLoadContent<Scene>(sceneName, out var scene)) {
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
        this.CurrentScene = scene;
    }

    /// <inheritdoc />
    public void PushScene(IScene scene) {
        if (Scene.IsNullOrEmpty(this.CurrentScene)) {
            scene.Initialize(this, this.CreateAssetManager());
        }
        else {
            this.CurrentScene.RaiseDeactivated();
            scene.Initialize(this, this.CurrentScene.Assets);
        }

        this._sceneStack.Push(scene);
        scene.RaiseActivated();
    }

    /// <inheritdoc />
    public void RaiseCultureChanged() {
        Resources.Culture = CultureInfo.GetCultureInfo(this.DisplaySettings.Culture.ToCultureName());
        this.CultureChanged.SafeInvoke(this, this.DisplaySettings.Culture);
    }

    /// <inheritdoc />
    public void SaveAndApplyUserSettings() {
        this.SaveUserSettings();
        this.ApplyDisplaySettings();
    }

    /// <inheritdoc />
    public void SaveUserSettings() {
        this.DataManager.Save(UserSettings.FileName, this.UserSettings);
        this.SettingsSaved.SafeInvoke(this);
    }

    /// <inheritdoc />
    public bool TryPopScene(out IScene scene) {
        if (this._sceneStack.Count > 1) {
            scene = this._sceneStack.Pop();
            scene.Deinitialize();
            this.CurrentScene.RaiseActivated();
            return true;
        }

        scene = EmptyObject.Scene;
        return false;
    }

    /// <summary>
    /// Creates a scene level asset manager.
    /// </summary>
    /// <returns>The asset manager.</returns>
    protected virtual IAssetManager CreateAssetManager() {
        var assetManager = new AssetManager();
        var contentManager = new ContentManager(this.Content.ServiceProvider, this.Content.RootDirectory);
        assetManager.Initialize(contentManager, Serializer.Instance);
        return assetManager;
    }

    /// <inheritdoc />
    protected override void Draw(GameTime gameTime) {
        if (this.GraphicsDevice != null && this.SpriteBatch != null) {
            var previousRenderTarget = this.GetGameRenderTarget(this.GraphicsDevice);
            this.GraphicsDevice.SetRenderTarget(previousRenderTarget);
            this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
            this.RenderScenes();
            
            if (this.Project.ScreenShaders.CheckHasEnabledShaders(this) && !IsDesignMode) {
                foreach (var shader in this.Project.ScreenShaders) {
                    if (shader.IsEnabled && !this.DisplaySettings.DisabledScreenShaders.Contains(shader.Id)) {
                        var renderSize = shader.GetRenderSize(this.ViewportSize, this.PixelRenderSize);
                        if (shader.Shader.PrepareAndGetShader(renderSize.ToVector2(), this, this.CurrentScene) is { } effect) {
                            var renderTarget = this.GetRenderTarget(this.GraphicsDevice, shader, renderSize);
                            this.GraphicsDevice.SetRenderTarget(renderTarget);
                            this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
                            this.SpriteBatch.Begin(effect: effect, samplerState: shader.SamplerStateType.ToSamplerState());
                            this.SpriteBatch.Draw(previousRenderTarget, renderTarget.Bounds, Color.White);
                            this.SpriteBatch.End();
                            previousRenderTarget = renderTarget;
                        }
                    }
                }
            }

            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
            this.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            this.SpriteBatch.Draw(previousRenderTarget, this.GraphicsDevice.Viewport.Bounds, Color.White);
            this.SpriteBatch.End();
        }
    }


    /// <inheritdoc />
    protected override void Initialize() {
        base.Initialize();

        this.ResetViewPort();

        if (IsDesignMode) {
            this.UserSettings = new UserSettings(this.Project);
        }
        else {
            this.DataManager.Initialize(this.Project.CompanyName, this.Project.Name);

            if (this.DataManager.TryLoad<UserSettings>(UserSettings.FileName, out var userSettings) && userSettings != null) {
                this.UserSettings = userSettings;
            }
            else {
                this.UserSettings = new UserSettings(this.Project);
            }

            this.State.Initialize(this.DataManager, this.UserSettings.Custom);

            if (this.InputBindings.DesiredInputDevice == InputDevice.Auto) {
                var gamePadState = GamePad.GetState(PlayerIndex.One);
                this.DesiredInputDevice = gamePadState.IsConnected ? InputDevice.GamePad : InputDevice.KeyboardMouse;
            }
            else {
                this.DesiredInputDevice = this.InputBindings.DesiredInputDevice;
            }
        }

        this.RaiseCultureChanged();
        this.CurrentScene.Initialize(this, this.CreateAssetManager());
        this.Overlay.Initialize(this, this.CreateAssetManager());

        var assetManager = this.CreateAssetManager();
        foreach (var shader in this.Project.ScreenShaders) {
            shader.Shader.Initialize(assetManager, this);
        }

        this.IsInitialized = true;
    }

    /// <inheritdoc />
    protected override void LoadContent() {
        base.LoadContent();
        var assetManager = this.CreateAssetManager();
        if (assetManager.TryLoadContent<GameProject>(GameProject.ProjectFileName, out var project)) {
            this.Project = project;
        }

        var startupSceneId = this.Project.StartupSceneId;
        if (this.LaunchArguments.HasFlag(LaunchArguments.DebugMode) && this.Project.StartupDebugSceneId != Guid.Empty) {
            startupSceneId = this.Project.StartupDebugSceneId;
        }

        if (assetManager.TryLoadContent<Scene>(startupSceneId, out var scene) && scene.TryClone(out var clone)) {
            this.LoadScene(clone);
        }

        if (assetManager.TryLoadContent<Scene>(this.Project.PersistentOverlaySceneId, out var overlay)) {
            this.Overlay = overlay;
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

            if ((keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt)) && keyboardState.IsKeyDown(Keys.Enter)) {
                if (this._canToggleFullscreen) {
                    this.ToggleFullscreen();
                    this._canToggleFullscreen = false;
                }
            }
            else {
                this._canToggleFullscreen = true;
            }
        }

        if (this.ViewportSize.X != this.GraphicsDevice.Viewport.Width || this.ViewportSize.Y != this.GraphicsDevice.Viewport.Height) {
            this.ResetViewPort();
        }

        this.UpdateInputState();
        this.FrameTime = new FrameTime(gameTime, this.GameSpeed);
        this.RunTransitions();
        this.CurrentScene.Update(this.FrameTime, this.InputState);
        this.Overlay.Update(this.FrameTime, this.InputState);
    }

    /// <summary>
    /// Updates the <see cref="InputState" />.
    /// </summary>
    protected virtual void UpdateInputState() {
        this.InputState = new InputState(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One), this.InputState);

        if (!IsDesignMode) {
            if (this.InputBindings.DesiredInputDevice == InputDevice.Auto) {
                if (this.DesiredInputDevice != InputDevice.GamePad && this.InputState.IsGamePadActive()) {
                    this.DesiredInputDevice = InputDevice.GamePad;
                }
                else if (this.DesiredInputDevice != InputDevice.KeyboardMouse && (this.InputState.IsKeyboardActive() || this.InputState.IsMouseActive())) {
                    this.DesiredInputDevice = InputDevice.KeyboardMouse;
                }
            }
            else {
                this.DesiredInputDevice = this.InputBindings.DesiredInputDevice;
            }
        }
    }

    private void ClearSceneStack() {
        foreach (var scene in this._sceneStack) {
            scene.Deinitialize();
        }

        this._sceneStack.Clear();
    }


    private RenderTarget2D CreateRenderTarget(GraphicsDevice device) {
        var width = device.PresentationParameters.BackBufferWidth;
        var height = device.PresentationParameters.BackBufferHeight;
        return new RenderTarget2D(
            device,
            width,
            height);
    }

    private RenderTarget2D CreateRenderTarget(GraphicsDevice device, Point renderSize) =>
        new(
            device,
            renderSize.X,
            renderSize.Y);

    private RenderTarget2D GetGameRenderTarget(GraphicsDevice device) => this._gameRenderTarget ??= this.CreateRenderTarget(device);

    private RenderTarget2D GetRenderTarget(GraphicsDevice device, ScreenShader shader, Point renderSize) {
        if (!this._screenShaderIdToRenderTargets.TryGetValue(shader.Id, out var renderTarget)) {
            renderTarget = this.CreateRenderTarget(device, renderSize);
            this._screenShaderIdToRenderTargets[shader.Id] = renderTarget;
        }

        return renderTarget;
    }

    private void RenderScenes() {
        foreach (var scene in this._sceneStack.Reverse()) {
            scene.Render(this.FrameTime, this.InputState);
        }

        this.Overlay.Render(this.FrameTime, this.InputState);
    }

    private void ResetViewPort() {
        this.ViewportSize = new Point(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
        var ratio = this.Project.GetPixelAgnosticRatio(this.Project.CommonViewHeight, this.ViewportSize.Y);
        this.PixelRenderSize = new Point((int)Math.Round(this.ViewportSize.X * ratio), (int)Math.Round(this.ViewportSize.Y * ratio));
        this.ViewportSizeChanged.SafeInvoke(this, this.ViewportSize);

        this._gameRenderTarget?.Dispose();
        this._gameRenderTarget = null;

        foreach (var shaderRenderTarget in this._screenShaderIdToRenderTargets.Values) {
            shaderRenderTarget.Dispose();
        }

        this._screenShaderIdToRenderTargets.Clear();
    }

    private void RunTransitions() {
        for (var i = this._runningTransitions.Count - 1; i >= 0; i--) {
            var transition = this._runningTransitions[i];
            transition.Update(this.FrameTime);

            if (transition.IsComplete) {
                this._runningTransitions.Remove(transition);
            }
        }
    }

    private void ToggleFullscreen() {
        this.DisplaySettings.DisplayMode = this.DisplaySettings.DisplayMode == DisplayMode.Windowed ? DisplayMode.Borderless : DisplayMode.Windowed;
        this.SaveAndApplyUserSettings();
    }

    private sealed class EmptyGame : IGame {
        public event EventHandler<ResourceCulture>? CultureChanged;
        public event EventHandler<double>? GameSpeedChanged;
        public event EventHandler<InputDevice>? InputDeviceChanged;
        public event EventHandler? SettingsSaved;
        public event EventHandler<Point>? ViewportSizeChanged;
        public AudioSettings AudioSettings => this.UserSettings.Audio;
        public ContentManager? Content => null;
        public IScene CurrentScene => EmptyObject.Scene;
        public IDataManager DataManager => EmptyDataManager.Instance;
        public InputDevice DesiredInputDevice => InputDevice.GamePad;
        public DisplaySettings DisplaySettings => this.UserSettings.Display;

        public double GameSpeed {
            get => 1f;
            set {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                this.GameSpeedChanged.SafeInvoke(this, 1f);
            }
        }

        public GraphicsDevice? GraphicsDevice => null;
        public InputBindings InputBindings => this.UserSettings.Input;
        public LaunchArguments LaunchArguments => LaunchArguments.None;
        public IScene Overlay => EmptyObject.Scene;
        public Point PixelRenderSize => default;
        public IGameProject Project => GameProject.Empty;
        public SpriteBatch? SpriteBatch => null;
        public GameState State { get; } = new();
        public UserSettings UserSettings { get; } = new();
        public Point ViewportSize => default;

        public void ApplyDisplaySettings() {
        }

        public void BeginTransition(GameTransition transition) {
        }

        public void Exit() {
        }

        public void LoadScene(string sceneName) {
        }

        public void LoadScene(IScene scene) {
        }

        public void PushScene(IScene scene) {
        }

        public void RaiseCultureChanged() {
        }

        public void SaveAndApplyUserSettings() {
        }

        public void SaveUserSettings() {
        }

        public bool TryPopScene(out IScene scene) {
            scene = EmptyObject.Scene;
            return false;
        }
    }
}