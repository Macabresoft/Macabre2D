namespace Macabre2D.Engine.Windows.Controls.SceneEditing {

    using Macabre2D.Engine.Windows.Models.FrameworkWrappers;
    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Framework.WpfInterop;
    using MonoGame.Framework.WpfInterop.Input;
    using System.ComponentModel;

    public class EditorGame : WpfGame, IGame, INotifyPropertyChanged {
        private readonly EditorCameraWrapper _cameraWrapper;
        private readonly SelectionEditor _selectionEditor;
        private IAssetManager _assetManager = new AssetManager();
        private IScene _currentScene;
#pragma warning disable IDE0052 // Remove unread private members. This is somehow used by the base class with reflection.
        private FrameTime _frameTime;
        private IGraphicsDeviceService _graphicsDeviceManager;
#pragma warning restore IDE0052 // Remove unread private members
        private bool _isContentLoaded = false;
        private bool _isInitialized = false;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private string _rootContentDirectory;
        private IGameSettings _settings;

        public EditorGame(EditorCameraWrapper cameraWrapper, SelectionEditor selectionEditor) : base() {
            this._cameraWrapper = cameraWrapper;
            this._selectionEditor = selectionEditor;
            this.Settings = new GameSettings();
            this.SizeChanged += this.EditorGame_SizeChanged;
            MacabreGame.Instance = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IAssetManager AssetManager {
            get {
                return this._assetManager;
            }

            set {
                if (value != null) {
                    this._assetManager = new EditorAssetManager(value);

                    if (this.Content != null) {
                        this._assetManager.Initialize(this.Content);
                    }

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.AssetManager)));
                }
            }
        }

        public Camera CurrentCamera {
            get {
                return this._cameraWrapper.Camera;
            }
        }

        public IScene CurrentScene {
            get {
                return this._currentScene;
            }

            set {
                this._currentScene = value;

                if (this._isInitialized && this._currentScene != null) {
                    this.InitializeComponents();
                }

                if (this._isContentLoaded) {
                    this._currentScene.LoadContent();
                }

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentScene)));
            }
        }

        public ComponentEditingStyle EditingStyle { get; internal set; } = ComponentEditingStyle.Translation;

        public GraphicsSettings GraphicsSettings { get; } = new GraphicsSettings();

        public bool InstantiatePrefabs {
            get {
                return false;
            }
        }

        public ISaveDataManager SaveDataManager { get; } = new EmptySaveDataManager();

        public IGameSettings Settings {
            get {
                return this._settings;
            }

            set {
                if (value != null) {
                    this._settings = value;
                    GameSettings.Instance = this._settings;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Settings)));
                }
            }
        }

        public bool ShowGrid { get; internal set; } = true;

        public bool ShowSelection { get; internal set; } = true;

        public SpriteBatch SpriteBatch { get; private set; }

        public void FocusComponent(BaseComponent component) {
            if (component is IBoundable boundable) {
                this._cameraWrapper.Camera.ZoomTo(boundable);
            }
            else if (component != null) {
                this._cameraWrapper.Camera.SetWorldPosition(component.WorldTransform.Position);
            }
        }

        public void ResetCamera() {
            // This probably seems weird, but it resets the view height which causes the view matrix
            // and bounding area to be reevaluated.
            this._cameraWrapper.Camera.ViewHeight += 1;
            this._cameraWrapper.Camera.ViewHeight -= 1;
        }

        public void SaveAndApplyGraphicsSettings() {
            return;
        }

        public void SetContentPath(string path) {
            this._rootContentDirectory = path;
            if (this.Content != null) {
                this.Content.RootDirectory = this._rootContentDirectory;
            }
        }

        protected override void Draw(GameTime gameTime) {
            if (this._isInitialized && this._isContentLoaded) {
                if (this.CurrentScene != null) {
                    this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
                    this.CurrentScene.Draw(this._frameTime, this._cameraWrapper.Camera);
                    this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, RasterizerState.CullNone, null, this._cameraWrapper.Camera.ViewMatrix);
                    this._cameraWrapper.Draw(this._frameTime);
                    this._selectionEditor.Draw(this._frameTime, this._cameraWrapper.Camera.BoundingArea);
                    this.SpriteBatch.End();
                }
                else if (this.Settings != null) {
                    this.GraphicsDevice.Clear(this.Settings.FallbackBackgroundColor);
                }
            }
        }

        protected override void Initialize() {
            this._graphicsDeviceManager = new WpfGraphicsDeviceService(this);
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
            this._keyboard = new WpfKeyboard(this);
            this._mouse = new WpfMouse(this);

            this.InitializeComponents();
            base.Initialize();
            this._isInitialized = true;
        }

        protected override void LoadContent() {
            this.Content.RootDirectory = this._rootContentDirectory;
            this.AssetManager.Initialize(this.Content);
            this.CurrentScene?.LoadContent();
            base.LoadContent();
            this._isContentLoaded = true;
        }

        protected override void Update(GameTime gameTime) {
            if (this.CurrentScene != null) {
                var mouseState = this._mouse.GetState();
                var keyboardState = this._keyboard.GetState();

                this._frameTime = new FrameTime(gameTime, 1f);
                this._selectionEditor.Update(this._frameTime, mouseState, keyboardState);
                this._cameraWrapper.Update(this._frameTime, mouseState, keyboardState);
            }
        }

        private void EditorGame_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
            if (this.IsInitialized && (e.NewSize.Width > e.PreviousSize.Width || e.NewSize.Height > e.PreviousSize.Height)) {
                this.ResetCamera();
            }
        }

        private void InitializeComponents() {
            if (this.CurrentScene != null) {
                this.CurrentScene.Initialize();
                this._cameraWrapper.Initialize(this);
                this._selectionEditor.Initialize(this);
            }
        }
    }
}