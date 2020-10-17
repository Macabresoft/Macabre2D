namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {

    using Avalonia.Controls;
    using Macabresoft.Macabre2D.AvaloniaUI;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public sealed class SolidViewModel : MonoGameViewModel {
        private CameraComponent _camera;

        public SolidViewModel(IAvaloniaGame game) : base(game) {
            this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        }

        public override void Initialize(Window window, Avalonia.Size viewportSize, MonoGameMouse mouse, MonoGameKeyboard keyboard) {
            if (this.Game.Settings is GameSettings settings) {
                settings.PixelsPerUnit = 64;
            }

            this.Game.LoadScene(new GameScene());
            this.Game.Scene.AddSystem<RenderSystem>();
            this.Game.Scene.AddSystem<UpdateSystem>();
            this.Game.Scene.BackgroundColor = DefinedColors.CosmicJam;

            var cameraEntity = this.Game.Scene.AddChild();
            this._camera = cameraEntity.AddComponent<CameraComponent>();
            this._camera.ViewHeight = 6f;

            base.Initialize(window, viewportSize, mouse, keyboard);
        }

        public void ResetCamera() {
            if (this._camera != null) {
                // This probably seems weird, but it resets the view height which causes the view
                // matrix and bounding area to be reevaluated.
                this._camera.ViewHeight += 1;
                this._camera.ViewHeight -= 1;
            }
        }

        private void Game_ViewportSizeChanged(object sender, Point e) {
            this.ResetCamera();
        }
    }
}