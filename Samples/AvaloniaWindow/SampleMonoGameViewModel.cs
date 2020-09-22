namespace Macabresoft.MonoGame.Samples.AvaloniaWindow {

    using Macabresoft.MonoGame.AvaloniaUI;
    using Macabresoft.MonoGame.Core;
    using Macabresoft.MonoGame.Samples.AvaloniaWindow.Components;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;

    public class SampleMonoGameViewModel : MonoGameViewModel, IGame {
        private CameraComponent _camera;

        public SampleMonoGameViewModel() : base() {
        }

        public override void Initialize(MonoGameMouse mouse) {
            if (this.Settings is GameSettings settings) {
                settings.PixelsPerUnit = 64;
            }

            var skullId = Guid.NewGuid();
            this.AssetManager.SetMapping(skullId, "skull");
            var spriteRenderer = this.Scene.AddChild().AddComponent<SpriteRenderComponent>();
            spriteRenderer.Sprite = new Sprite(skullId);
            spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;

            var leageMonoId = Guid.NewGuid();
            this.AssetManager.SetMapping(leageMonoId, "League Mono");

            var textRenderEntity = this.Scene.AddChild();
            var textRenderer = textRenderEntity.AddComponent<TextRenderComponent>();
            textRenderer.Font = new Font(leageMonoId);
            textRenderer.Text = @"github.com/Macabresoft/Macabresoft.MonoGame";
            textRenderer.Color = DefinedColors.MacabresoftYellow;
            textRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            textRenderEntity.LocalScale = new Vector2(0.25f);
            textRenderEntity.LocalPosition = new Vector2(0f, -3.5f);

            var cameraEntity = this.Scene.AddChild();
            this._camera = cameraEntity.AddComponent<CameraComponent>();
            this._camera.ViewHeight = 6f;
            var cameraChild = cameraEntity.AddChild();
            var cameraTextRenderer = cameraChild.AddComponent<TextRenderComponent>();
            cameraTextRenderer.Font = new Font(leageMonoId);
            cameraTextRenderer.Color = DefinedColors.MacabresoftBone;
            cameraTextRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            cameraTextRenderer.Text = "Mouse Position: (0.00, 0.00)";
            cameraChild.AddComponent<MouseDebuggerComponent>();
            cameraChild.LocalScale = new Vector2(0.1f);
            cameraChild.LocalPosition = new Vector2(0f, -3f);

            this.Scene.AddSystem<UpdateSystem>();

            base.Initialize(mouse);
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

            if (this.IsInitialized && this.Scene != null) {
                this.Scene.Update(this.FrameTime, this.InputState);

                if (this.InputState.CurrentMouseState.LeftButton == ButtonState.Pressed && this.InputState.PreviousMouseState.LeftButton == ButtonState.Released) {
                    this.Scene.BackgroundColor = DefinedColors.MacabresoftBlack;
                }
                else if (this.InputState.CurrentMouseState.LeftButton == ButtonState.Released && this.InputState.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                    this.Scene.BackgroundColor = DefinedColors.MacabresoftPurple;
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