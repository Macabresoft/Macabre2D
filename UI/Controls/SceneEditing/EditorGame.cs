namespace Macabre2D.UI.Controls.SceneEditing {

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
        private IGraphicsDeviceService _graphicsDeviceManager;
#pragma warning restore IDE0052 // Remove unread private members
        private bool _isContentLoaded = false;
        private bool _isInitialized = false;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private IGameSettings _settings;

        public EditorGame(EditorCameraWrapper cameraWrapper, SelectionEditor selectionEditor) : base() {
            this._cameraWrapper = cameraWrapper;
            this._selectionEditor = selectionEditor;
            this.Settings = new GameSettings();
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
                return this._cameraWrapper?.Camera;
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

        public bool ShowRotationGizmo { get; internal set; }

        public bool ShowScaleGizmo { get; internal set; }

        public bool ShowSelection { get; internal set; } = true;

        public bool ShowTranslationGizmo { get; internal set; } = true;

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

        public void SetContentPath(string path) {
            this.Content.RootDirectory = path;
        }

        protected override void Draw(GameTime gameTime) {
            if (this._isInitialized && this._isContentLoaded) {
                if (this.CurrentScene != null) {
                    this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
                    this.CurrentScene.Draw(gameTime, this._cameraWrapper.Camera);
                    this.CurrentScene.Game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, RasterizerState.CullClockwise, null, this._cameraWrapper.Camera.ViewMatrix);
                    this._cameraWrapper.Draw(gameTime);

                    if (this.ShowSelection) {
                        this._selectionEditor.Draw(gameTime, this._cameraWrapper.Camera.ViewHeight);
                    }

                    this.CurrentScene.Game.SpriteBatch.End();
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
            this.AssetManager.Initialize(this.Content);
            this.CurrentScene?.LoadContent();
            base.LoadContent();
            this._isContentLoaded = true;
        }

        protected override void Update(GameTime gameTime) {
            if (this.CurrentScene != null) {
                var mouseState = this._mouse.GetState();
                this._selectionEditor.Update(gameTime, mouseState);
                var keyboardState = this._keyboard.GetState();
                this._cameraWrapper.Update(gameTime, mouseState, keyboardState);
            }
        }

        private void InitializeComponents() {
            if (this.CurrentScene != null) {
                this.CurrentScene.Initialize(this);
                this._cameraWrapper.Initialize(this);
                this._selectionEditor.Initialize(this);
            }
        }
    }
}