using Macabresoft.Core;
using Macabresoft.MonoGame.AvaloniaUI;
using Macabresoft.MonoGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Macabresoft.MonoGame.Samples.AvaloniaWindow {

    public class SampleMonoGameViewModel : MonoGameViewModel, IGame {
        private CameraComponent _camera;
        private FrameTime _frameTime;
        private bool _isInitialized;

        public SampleMonoGameViewModel() {
            DefaultGame.Instance = this;
        }

        public event EventHandler<double> GameSpeedChanged;

        public event EventHandler<Point> ViewportSizeChanged;

        public IAssetManager AssetManager { get; } = new AssetManager();

        public double GameSpeed { get => 1d; set { return; } }

        public GraphicsSettings GraphicsSettings { get; } = new GraphicsSettings();

        public bool IsDesignMode => true;

        public ISaveDataManager SaveDataManager => Core.SaveDataManager.Empty;

        public IGameScene Scene { get; } = new GameScene();

        public IGameSettings Settings { get; } = new GameSettings();

        public SpriteBatch SpriteBatch { get; private set; }

        public Point ViewportSize { get; private set; }

        public override void Draw(GameTime gameTime) {
            if (this._isInitialized && this.Scene != null) {
                this.GraphicsDevice.Clear(this.Scene.BackgroundColor);
                this.Scene.Render(this._frameTime, new InputState());
                this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, RasterizerState.CullNone, null, this._camera.ViewMatrix);

                this.SpriteBatch.End();
            }
        }

        public void Exit() {
            return;
        }

        public override void Initialize() {
            base.Initialize();
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Scene.BackgroundColor = DefinedColors.MacabresoftPurple;
            this._camera = this.Scene.AddChild().AddComponent<CameraComponent>();
            this.Scene.Initialize(this);
            this._isInitialized = true;
        }

        public void LoadScene(string sceneName) {
            return;
        }

        public void LoadScene(IGameScene scene) {
            return;
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

        public override void SizeChanged(Avalonia.Size newSize) {
            var originalSize = this.ViewportSize;
            this.ViewportSize = new Point(Convert.ToInt32(newSize.Width), Convert.ToInt32(newSize.Height));
            this.ViewportSizeChanged.SafeInvoke(this, this.ViewportSize);

            if (newSize.Width > originalSize.X || newSize.Height > originalSize.Y) {
                this.ResetCamera();
            }
        }

        public override void Update(GameTime gameTime) {
            if (this.Scene != null) {
                this._frameTime = new FrameTime(gameTime, this.GameSpeed);
            }
        }
    }
}