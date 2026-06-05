namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Macabre2D.Common;
using Macabre2D.Project.Common;
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

    private readonly Stack<IScene> _sceneStack = new();

    private readonly FilterCollection<IUpdateableGameObject> _updateSystems = new(
        a => a.ShouldUpdate,
        (a, handler) => a.ShouldUpdateChanged += handler,
        (a, handler) => a.ShouldUpdateChanged -= handler);

    private bool _canToggleFullscreen = true;
    private Rectangle _finalRenderBounds;
    private RenderTarget2D? _gameRenderTarget;
    private GameTime _gameTime = new();

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
    public IDataManager DataManager { get; } = new DesktopDataManager();

    /// <inheritdoc />
    public DisplaySettings DisplaySettings => this.UserSettings.Display;

    /// <inheritdoc />
    public IInputActionIconResolver InputActionIconResolver { get; } = new InputActionIconResolver();

    /// <inheritdoc />
    public InputSettings InputSettings => this.UserSettings.Input;

    /// <inheritdoc />
    public LaunchArguments LaunchArguments { get; }

    public ICommonMeasurements Measurements { get; } = new CommonMeasurements();

    public IReadOnlyCollection<IScene> OpenScenes => this._sceneStack;

    /// <inheritdoc />
    public GameState State { get; } = new();

    /// <inheritdoc />
    public IReadOnlyCollection<IGameSystem> Systems => this.Project.Systems;

    /// <inheritdoc />
    public TimeSpan TimeSinceGameStart => this._gameTime.TotalGameTime;

    /// <inheritdoc />
    public Point CroppedViewportSize { get; private set; }

    /// <inheritdoc />
    public bool CurrentCultureRendersTextInScreenSpace { get; private set; }

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
    public InputDevice DesiredInputDevice {
        get;
        protected set {
            if (value != field && value != InputDevice.Auto) {
                field = value;
                this.InputDeviceChanged.SafeInvoke(this, field);
            }
        }
    } = InputDevice.Auto;

    /// <inheritdoc />
    public FrameTime FrameTime { get; private set; }

    /// <inheritdoc />
    public double GameSpeed {
        get;

        set {
            if (value >= 0f && Math.Abs(field - value) > 0.001f) {
                field = value;
                this.GameSpeedChanged.SafeInvoke(this, field);
            }
        }
    } = 1d;

    /// <inheritdoc />
    public InputState InputState { get; protected set; }

    /// <summary>
    /// Gets or sets a value which indicates whether the game is running in design mode.
    /// </summary>
    public static bool IsDesignMode { get; private set; }

    /// <inheritdoc />
    public IGameProject Project {
        get;
        protected set {
            field = value;
            this.ApplyDisplaySettings();
        }
    } = new GameProject();

    /// <inheritdoc />
    public SpriteBatch? SpriteBatch { get; private set; }

    /// <inheritdoc />
    public UserSettings UserSettings {
        get;
        private set {
            field = value;
            this.ApplyDisplaySettings();
        }
    } = new();

    /// <inheritdoc />
    public Point ViewportSize { get; private set; }

    /// <summary>
    /// Gets the graphics device manager.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected GraphicsDeviceManager GraphicsDeviceManager { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is initialized.
    /// </summary>
    protected bool IsInitialized { get; private set; }

    /// <inheritdoc />
    public T AddSystem<T>() where T : IGameSystem, new() {
        var system = new T();
        this.AddSystem(system);
        return system;
    }

    /// <inheritdoc />
    public void AddSystem(IGameSystem system) {
        this.Project.Systems.Add(system);
        this._updateSystems.Add(system);

        if (this.IsInitialized) {
            system.Initialize(this);
        }
    }

    /// <inheitdoc />
    public void ApplyDisplaySettings() {
        if (this.DisplaySettings.DisplayMode == DisplayMode.Borderless) {
            this.GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.Window.IsBorderless = true;
            this.GraphicsDeviceManager.IsFullScreen = false;
        }
        else {
            var resolution = this.DisplaySettings.GetResolution(this.Project.InternalRenderResolution);
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

        // This prevents the mouse position from being seen as "changed" next frame.
        // If the mouse thinks it has changed, the input device can change from game
        // pad to mouse / keyboard. We don't want that!
        this.InputState = this.CreateInputStateForFrame();
    }

    /// <inheritdoc />
    public T GetOrAddSystem<T>() where T : class, IGameSystem, new() => this.GetSystem<T>() ?? this.AddSystem<T>();

    /// <inheritdoc />
    public T? GetSystem<T>() where T : class, IGameSystem => this.Systems.OfType<T>().FirstOrDefault();

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
    public void LoadScene(Guid sceneId) {
        var assets = this.CreateAssetManager();

        if (assets.TryLoadContent<Scene>(sceneId, out var scene)) {
            this.LoadScene(scene);
        }
        else {
            throw new FileNotFoundException($"Could not find a scene with the ID: {sceneId}");
        }
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
        this.CurrentCultureRendersTextInScreenSpace = this.Project.Fonts.CheckShouldRenderInScreenSpace(this.DisplaySettings.Culture);
        this.CultureChanged.SafeInvoke(this, this.DisplaySettings.Culture);
    }

    /// <inheritdoc />
    public void ReloadUserSettings() {
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

            this.State.Initialize(this.DataManager, this.UserSettings.Gameplay);

            if (this.InputSettings.DesiredInputDevice == InputDevice.Auto) {
                var gamePadState = GamePad.GetState(PlayerIndex.One);
                this.DesiredInputDevice = gamePadState.IsConnected ? InputDevice.GamePad : InputDevice.KeyboardMouse;
            }
            else {
                this.DesiredInputDevice = this.InputSettings.DesiredInputDevice;
            }
        }
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
        if (this.SpriteBatch != null) {
            var renderTarget = this.GetGameRenderTarget(this.GraphicsDevice);
            this.GraphicsDevice.SetRenderTarget(renderTarget);
            this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);

            renderTarget = this.PerformRenderSteps(renderTarget, this.SpriteBatch);

            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
            this.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            this.SpriteBatch.Draw(renderTarget, this._finalRenderBounds, Color.White);
            this.SpriteBatch.End();
        }
    }

    /// <inheritdoc />
    protected override void Initialize() {
        base.Initialize();

        this.ResetViewPort();
        this.ReloadUserSettings();
        this.Measurements.Initialize(this, this.Project);

        this._updateSystems.Clear();
        this._updateSystems.AddRange(this.Systems);

        this.InputActionIconResolver.Initialize(this);
        this.RaiseCultureChanged();

        var assetManager = this.CreateAssetManager();
        foreach (var renderStep in this.Project.RenderSteps) {
            renderStep.Initialize(assetManager, this);
        }

        foreach (var system in this.Project.Systems) {
            system.Initialize(this);
        }

        this.CurrentScene.Initialize(this, this.CreateAssetManager());

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

        this.TryCreateSpriteBatch();

        if (!IsDesignMode) {
            assetManager.Dispose();
        }
    }

    /// <inheritdoc />
    protected override void OnExiting(object sender, ExitingEventArgs args) {
        base.OnExiting(sender, args);
        this.Measurements.Deinitialize();
    }

    /// <summary>
    /// Steps through the project's render steps one at a time.
    /// </summary>
    /// <param name="renderTarget">The render target.</param>
    /// <param name="spriteBatch">The sprite batch.</param>
    protected virtual RenderTarget2D PerformRenderSteps(RenderTarget2D renderTarget, SpriteBatch spriteBatch) {
        foreach (var step in this.Project.RenderSteps) {
            if (step.IsEnabled && !this.DisplaySettings.DisabledRenderSteps.Contains(step.Id)) {
                renderTarget = step.RenderToTexture(spriteBatch, renderTarget);
            }
        }

        return renderTarget;
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
            this._gameTime = gameTime;
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

        this._updateSystems.RebuildCache();
        foreach (var system in this._updateSystems) {
            system.Update(this.FrameTime, this.InputState);
        }

        this.CurrentScene.Update(this.FrameTime, this.InputState);
    }

    /// <summary>
    /// Updates the <see cref="InputState" />.
    /// </summary>
    protected virtual void UpdateInputState() {
        this.InputState = this.CreateInputStateForFrame();

        if (!IsDesignMode) {
            if (this.InputSettings.DesiredInputDevice == InputDevice.Auto) {
                if (this.DesiredInputDevice != InputDevice.GamePad && this.InputState.IsGamePadActive()) {
                    this.DesiredInputDevice = InputDevice.GamePad;
                }
                else if (this.DesiredInputDevice != InputDevice.KeyboardMouse && (this.InputState.IsKeyboardActive() || this.InputState.IsMouseActive())) {
                    this.DesiredInputDevice = InputDevice.KeyboardMouse;
                }
            }
            else {
                this.DesiredInputDevice = this.InputSettings.DesiredInputDevice;
            }
        }
    }

    private void ClearSceneStack() {
        foreach (var scene in this._sceneStack) {
            scene.Deinitialize();
        }

        this._sceneStack.Clear();
    }

    private InputState CreateInputStateForFrame() => new(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One), this.InputState);

    private RenderTarget2D GetGameRenderTarget(GraphicsDevice device) => this._gameRenderTarget ??= device.CreateRenderTarget(this.Project);

    private void ResetViewPort() {
        this.ViewportSize = new Point(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);

        if (this.Project.InternalRenderResolution.X > 0f && this.Project.InternalRenderResolution.Y > 0f && this.ViewportSize.X > 0f && this.ViewportSize.Y > 0f) {
            var internalRatio = (double)this.Project.InternalRenderResolution.X / this.Project.InternalRenderResolution.Y;
            var viewPortRatio = (double)this.ViewportSize.X / this.ViewportSize.Y;
            var width = this.ViewportSize.X;
            var height = this.ViewportSize.Y;
            var horizontalOffset = 0;
            var verticalOffset = 0;

            if (internalRatio > viewPortRatio) {
                // The view port is more vertical than the internal render resolution  and requires horizontal black bars
                height = (int)Math.Ceiling(width / internalRatio);
                verticalOffset = (this.ViewportSize.Y - height) / 2;
            }
            else if (internalRatio < viewPortRatio) {
                // The view port is more horizontal than the internal render resolution and requires vertical black bars
                width = (int)Math.Ceiling(height * internalRatio);
                horizontalOffset = (this.ViewportSize.X - width) / 2;
            }

            this.CroppedViewportSize = new Point(width, height);
            this._finalRenderBounds = new Rectangle(horizontalOffset, verticalOffset, width, height);
        }
        else {
            this.CroppedViewportSize = this.ViewportSize;
        }

        this.ViewportSizeChanged.SafeInvoke(this, this.ViewportSize);

        this._gameRenderTarget?.Dispose();
        this._gameRenderTarget = null;

        foreach (var step in this.Project.RenderSteps) {
            step.Reset();
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
        public Point CroppedViewportSize => this.ViewportSize;
        public bool CurrentCultureRendersTextInScreenSpace => false;
        public IScene CurrentScene => EmptyObject.Scene;
        public IDataManager DataManager => EmptyDataManager.Instance;
        public InputDevice DesiredInputDevice => InputDevice.GamePad;
        public DisplaySettings DisplaySettings => this.UserSettings.Display;
        public FrameTime FrameTime => FrameTime.Zero;
        public GraphicsDevice? GraphicsDevice => null;
        public IInputActionIconResolver InputActionIconResolver => Framework.InputActionIconResolver.Empty;
        public InputSettings InputSettings => this.UserSettings.Input;
        public LaunchArguments LaunchArguments => LaunchArguments.None;
        public ICommonMeasurements Measurements => EmptyObject.Instance;
        public IReadOnlyCollection<IScene> OpenScenes { get; } = [];
        public IGameProject Project => GameProject.Empty;
        public SpriteBatch? SpriteBatch => null;
        public GameState State { get; } = new();
        public IReadOnlyCollection<IGameSystem> Systems { get; } = [];
        public TimeSpan TimeSinceGameStart => TimeSpan.Zero;
        public UserSettings UserSettings { get; } = new();
        public Point ViewportSize => default;

        public double GameSpeed {
            get => 1f;
            set {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                this.GameSpeedChanged.SafeInvoke(this, 1f);
            }
        }

        public T AddSystem<T>() where T : IGameSystem, new() => new();

        public void AddSystem(IGameSystem system) {
        }

        public void ApplyDisplaySettings() {
        }

        public void Exit() {
        }

        public T GetOrAddSystem<T>() where T : class, IGameSystem, new() => new();

        public T? GetSystem<T>() where T : class, IGameSystem => null;

        public void LoadScene(string sceneName) {
        }

        public void LoadScene(IScene scene) {
        }

        public void LoadScene(Guid sceneId) {
        }

        public void PushScene(IScene scene) {
        }

        public void RaiseCultureChanged() {
        }

        public void ReloadUserSettings() {
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