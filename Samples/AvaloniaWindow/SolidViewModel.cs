namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {
    using Avalonia;
    using Avalonia.Controls;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;
    using Point = Microsoft.Xna.Framework.Point;

    public sealed class SolidViewModel : MonoGameViewModel {
        private Camera _camera;

        public SolidViewModel(IAvaloniaGame game) : base(game) {
            this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        }

        public override void Initialize(Window window, Size viewportSize, MonoGameMouse mouse, MonoGameKeyboard keyboard) {
            this.Game.LoadScene(new Scene());
            this.Game.Scene.AddSystem<RenderSystem>();
            this.Game.Scene.AddSystem<UpdateSystem>();
            this.Game.Scene.BackgroundColor = DefinedColors.CosmicJam;

            this._camera = this.Game.Scene.AddChild<Camera>();
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