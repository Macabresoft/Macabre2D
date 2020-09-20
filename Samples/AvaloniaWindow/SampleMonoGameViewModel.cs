namespace Macabresoft.MonoGame.Samples.AvaloniaWindow {

    using Macabresoft.MonoGame.AvaloniaUI;
    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using System;

    public class SampleMonoGameViewModel : MonoGameViewModel, IGame {
        private CameraComponent _camera;

        public SampleMonoGameViewModel() : base() {
        }

        public override void Initialize() {
            base.Initialize();
            this.Scene.BackgroundColor = DefinedColors.CosmicJam;
            this._camera = this.Scene.AddChild().AddComponent<CameraComponent>();
        }

        public void ResetCamera() {
            // This probably seems weird, but it resets the view height which causes the view matrix
            // and bounding area to be reevaluated.
            this._camera.ViewHeight += 1;
            this._camera.ViewHeight -= 1;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (this.Scene != null) {
                if ((int)Math.Round(this.FrameTime.TotalTimeSpan.TotalSeconds) % 2 == 0) {
                    this.Scene.BackgroundColor = DefinedColors.MacabresoftRed;
                }
                else {
                    this.Scene.BackgroundColor = DefinedColors.MacabresoftYellow;
                }
            }
        }

        protected override void OnViewportChanged(Point originalSize, Point newSize) {
            base.OnViewportChanged(originalSize, newSize);

            if (newSize.X > originalSize.X || newSize.Y > originalSize.Y) {
                this.ResetCamera();
            }
        }
    }
}