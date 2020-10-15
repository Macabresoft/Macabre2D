namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {

    using Avalonia.Controls;
    using Macabresoft.Macabre2D.AvaloniaUI;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.Samples.AvaloniaWindow.Components;
    using Microsoft.Xna.Framework;
    using System;

    public class SampleMonoGameViewModel : MonoGameViewModel {
        private CameraComponent _camera;
        private SpriteRenderComponent _skullRenderer;

        public SampleMonoGameViewModel() : base() {
            this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        }

        public override void Initialize(Window window, Avalonia.Size viewportSize, MonoGameMouse mouse, MonoGameKeyboard keyboard) {
            if (this.Game.Settings is GameSettings settings) {
                settings.PixelsPerUnit = 64;
            }

            this.Game.LoadScene(new GameScene());
            this.Game.Scene.AddSystem<RenderSystem>();
            this.Game.Scene.AddSystem<UpdateSystem>();
            this.Game.Scene.BackgroundColor = DefinedColors.MacabresoftPurple;

            var skullId = Guid.NewGuid();
            this.Game.AssetManager.SetMapping(skullId, "skull");
            var skullEntity = this.Game.Scene.AddChild();
            this._skullRenderer = skullEntity.AddComponent<SpriteRenderComponent>();
            this._skullRenderer.Sprite = new Sprite(skullId);
            this._skullRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            skullEntity.AddComponent<SampleInputComponent>();

            var leageMonoId = Guid.NewGuid();
            this.Game.AssetManager.SetMapping(leageMonoId, "League Mono");

            var textRenderEntity = this.Game.Scene.AddChild();
            var textRenderer = textRenderEntity.AddComponent<TextRenderComponent>();
            textRenderer.Font = new Font(leageMonoId);
            textRenderer.Text = @"github.com/Macabresoft/Macabresoft.Macabre2D";
            textRenderer.Color = DefinedColors.MacabresoftYellow;
            textRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            textRenderEntity.LocalScale = new Vector2(0.25f);
            textRenderEntity.LocalPosition = new Vector2(0f, -3.5f);

            var cameraEntity = this.Game.Scene.AddChild();
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
            var frameRateDisplayEntity = cameraEntity.AddChild();
            var frameRateDisplay = frameRateDisplayEntity.AddComponent<FrameRateDisplayComponent>();
            frameRateDisplay.Font = new Font(leageMonoId);
            frameRateDisplay.Color = DefinedColors.ZvukostiGreen;
            frameRateDisplayEntity.LocalScale = new Vector2(0.1f);

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