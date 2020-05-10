namespace Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.MonoGameIntegration;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Windows;

    public class SongEditor : MonoGameViewModel, IGame {
        private readonly Camera _camera;
        private readonly IPianoRoll _pianoRoll = new PianoRoll();
        private FrameTime _frameTime;
        private bool _isContentLoaded = false;
        private bool _isInitialized = false;

        public SongEditor() : base() {
            this.Settings = new GameSettings() {
                PixelsPerUnit = 16
            };

            GameSettings.Instance = this.Settings;
            MacabreGame.Instance = this;
            this.AssetManager.SetMapping(Guid.NewGuid(), PianoRoll.SpriteSheetPath);
            this.CurrentScene = new Scene {
                BackgroundColor = Color.Black
            };

            this._camera = this.CurrentScene.AddChild<Camera>();
            this._camera.ViewHeight = 36f;
            this.CurrentScene.AddChild(new PianoComponent(this._pianoRoll));
        }

        public event EventHandler<double> GameSpeedChanged;

        public IAssetManager AssetManager { get; } = new AssetManager();

        public IScene CurrentScene { get; }

        public double GameSpeed {
            get {
                return 1f;
            }

            set {
                this.GameSpeedChanged.SafeInvoke(this, 1f);
            }
        }

        public GraphicsSettings GraphicsSettings { get; } = new GraphicsSettings();

        public bool IsDesignMode {
            get {
                return true;
            }
        }

        public ISaveDataManager SaveDataManager { get; } = new EmptySaveDataManager();

        public IGameSettings Settings { get; }

        public bool ShowGrid { get; internal set; } = true;

        public bool ShowSelection { get; internal set; } = true;

        public SpriteBatch SpriteBatch { get; private set; }

        public override void Draw(GameTime gameTime) {
            if (this._isInitialized && this._isContentLoaded) {
                this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
                this.CurrentScene.Draw(this._frameTime);
            }
        }

        public void Exit() {
            return;
        }

        public override void Initialize(MonoGameKeyboard keyboard, MonoGameMouse mouse) {
            base.Initialize(keyboard, mouse);
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.AssetManager.Initialize(this.Content);
            this.CurrentScene.Initialize();
            this._isInitialized = true;
        }

        public override void LoadContent() {
            this.CurrentScene.LoadContent();
            this._isContentLoaded = true;
        }

        public void ResetCamera() {
            // This probably seems weird, but it resets the view height which causes the view matrix
            // and bounding area to be reevaluated.
            this._camera.ViewHeight += 1;
            this._camera.ViewHeight -= 1;
        }

        public void SaveAndApplyGraphicsSettings() {
            return;
        }

        public override void SizeChanged(object sender, SizeChangedEventArgs e) {
            if (e.NewSize.Width > e.PreviousSize.Width || e.NewSize.Height > e.PreviousSize.Height) {
                this.ResetCamera();
            }
        }

        public override void Update(GameTime gameTime) {
            this._frameTime = new FrameTime(gameTime, this.GameSpeed);
            this.CurrentScene.Update(this._frameTime);
        }
    }
}