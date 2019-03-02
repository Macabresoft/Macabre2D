namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Framework.WpfInterop;
    using MonoGame.Framework.WpfInterop.Input;
    using System.ComponentModel;

    public class EditorGame : WpfGame, IGame, INotifyPropertyChanged {
        private readonly EditorCameraWrapper _cameraWrapper;
        private readonly SelectionEditor _selectionEditor;
        private readonly IComponentSelectionService _selectionService;
        private IScene _currentScene;
        private GameSettings _gameSettings = new GameSettings();
        private IGraphicsDeviceService _graphicsDeviceManager;
        private bool _isContentLoaded = false;
        private bool _isInitialized = false;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;

        public EditorGame() : base() {
            this._selectionService = ViewContainer.Resolve<IComponentSelectionService>();
            this._cameraWrapper = new EditorCameraWrapper(this);
            this._selectionEditor = new SelectionEditor(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICamera CurrentCamera {
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

        public GameSettings GameSettings {
            get {
                return this._gameSettings;
            }

            set {
                if (value != null) {
                    this._gameSettings = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.GameSettings)));
                }
            }
        }

        public bool ShowGrid { get; internal set; }

        public bool ShowSelection { get; internal set; }

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
            else if (this.GameSettings != null) {
                this.GraphicsDevice.Clear(this.GameSettings.FallbackBackgroundColor);
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
                this._cameraWrapper.RefreshCamera();
                this._cameraWrapper.Initialize();
                this._selectionEditor.Reinitialize();
            }
        }
    }
}