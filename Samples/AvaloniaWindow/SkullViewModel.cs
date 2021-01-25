using ReactiveUI;

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
            get { return this._displayText; }

            set {
                this.RaiseAndSetIfChanged(ref this._displayText, value);
                if (this._displayTextRenderer != null) {
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

            var gridComponent = this.Game.Scene.AddChild().AddComponent<GridDrawerComponent>();
            gridComponent.Color = DefinedColors.MacabresoftBone * 0.5f;
            gridComponent.UseDynamicLineThickness = false;
            gridComponent.Grid = new TileGrid(Vector2.One);
            gridComponent.RenderOrder = -1;

            var skull = new SpriteSheet();
            this.Game.AssetManager.SetContentMapping(skull.ContentId, "skull");
            this.Game.AssetManager.AddAsset(skull);
            
            var skullEntity = this.Game.Scene.AddChild();
            skullEntity.LocalPosition += new Vector2(0f, 0.5f);
            this._skullRenderer = skullEntity.AddComponent<SpriteRenderComponent>();
            this._skullRenderer.SpriteReference.AssetId = skull.AssetId;
            this._skullRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            skullEntity.AddComponent<SampleInputComponent>();

            var leageMono = new Font();
            this.Game.AssetManager.SetContentMapping(leageMono.ContentId, "League Mono");
            this.Game.AssetManager.AddAsset(leageMono);

            var textRenderEntity = this.Game.Scene.AddChild();
            this._displayTextRenderer = textRenderEntity.AddComponent<TextRenderComponent>();
            this._displayTextRenderer.FontReference.AssetId = leageMono.AssetId;
            this._displayTextRenderer.Color = DefinedColors.MacabresoftYellow;
            this._displayTextRenderer.Text = this.DisplayText;
            this._displayTextRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            textRenderEntity.LocalScale = new Vector2(0.25f);
            textRenderEntity.LocalPosition = new Vector2(0f, -3f);

            var cameraEntity = this.Game.Scene.AddChild();
            this._camera = cameraEntity.AddComponent<CameraComponent>();
            this._camera.ViewHeight = 9f;
            var cameraChild = cameraEntity.AddChild();
            var cameraTextRenderer = cameraChild.AddComponent<TextRenderComponent>();
            cameraTextRenderer.FontReference.AssetId = leageMono.AssetId;
            cameraTextRenderer.Color = DefinedColors.MacabresoftBone;
            cameraTextRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            cameraTextRenderer.Text = "Mouse Position: (0.00, 0.00)";
            cameraChild.AddComponent<MouseDebuggerComponent>();
            cameraChild.LocalScale = new Vector2(0.1f);
            cameraChild.LocalPosition = new Vector2(0f, -2.5f);
            var frameRateDisplayEntity = cameraEntity.AddChild();
            var frameRateDisplay = frameRateDisplayEntity.AddComponent<FrameRateDisplayComponent>();
            frameRateDisplay.FontReference.AssetId = leageMono.AssetId;
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