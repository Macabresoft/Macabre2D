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
            if (this.Settings is GameSettings settings) {
                settings.PixelsPerUnit = 64;
            }

            var skullId = Guid.NewGuid();
            this.AssetManager.SetMapping(skullId, "skull");
            var spriteRenderer = this.Scene.AddChild().AddComponent<SpriteRenderComponent>();
            spriteRenderer.Sprite = new Sprite(skullId);
            spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            this._camera = this.Scene.AddChild().AddComponent<CameraComponent>();
            base.Initialize();
            this.Scene.BackgroundColor = DefinedColors.MacabresoftPurple;
        }

        public void ResetCamera() {
            // This probably seems weird, but it resets the view height which causes the view matrix
            // and bounding area to be reevaluated.
            this._camera.ViewHeight += 1;
            this._camera.ViewHeight -= 1;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        protected override void OnViewportChanged(Point originalSize, Point newSize) {
            base.OnViewportChanged(originalSize, newSize);

            if (newSize.X > originalSize.X || newSize.Y > originalSize.Y) {
                this.ResetCamera();
            }
        }
    }
}