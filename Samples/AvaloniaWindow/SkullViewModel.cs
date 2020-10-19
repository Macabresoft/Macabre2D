namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {

    using Avalonia.Controls;
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.Samples.AvaloniaWindow.Components;
    using Microsoft.Xna.Framework;
    using System;

    public sealed class SkullViewModel : MonoGameViewModel {
        private CameraComponent _camera;
        private string _displayText = @"github.com/Macabresoft/Macabresoft.Macabre2D";
        private TextRenderComponent _displayTextRenderer;
        private SpriteRenderComponent _skullRenderer;

        public SkullViewModel(IAvaloniaGame game) : base(game) {
            this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        }

        public string DisplayText {
            get {
                return this._displayText;
            }

            set {
                if (this.Set(ref this._displayText, value) && this._displayTextRenderer != null) {
                    this._displayTextRenderer.Text = value;
                }
            }
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
            this._displayTextRenderer = textRenderEntity.AddComponent<TextRenderComponent>();
            this._displayTextRenderer.Font = new Font(leageMonoId);
            this._displayTextRenderer.Color = DefinedColors.MacabresoftYellow;
            this._displayTextRenderer.Text = this.DisplayText;
            this._displayTextRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
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