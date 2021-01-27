namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {
    using Avalonia;
    using Avalonia.Controls;
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.Samples.AvaloniaWindow.Components;
    using Microsoft.Xna.Framework;
    using ReactiveUI;
    using Point = Microsoft.Xna.Framework.Point;

    public sealed class SkullViewModel : MonoGameViewModel {
        private CameraComponent _camera;
        private string _displayText = @"github.com/Macabresoft/Macabresoft.Macabre2D";
        private TextRenderComponent _displayTextRenderer;
        private SpriteRenderComponent _skullRenderer;

        public SkullViewModel(IAvaloniaGame game) : base(game) {
            this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        }

        public string DisplayText {
            get => this._displayText;

            set {
                this.RaiseAndSetIfChanged(ref this._displayText, value);
                if (this._displayTextRenderer != null) {
                    this._displayTextRenderer.Text = value;
                }
            }
        }

        public override void Initialize(Window window, Size viewportSize, MonoGameMouse mouse, MonoGameKeyboard keyboard) {
            this.Game.Project.Settings.PixelsPerUnit = 32;

            var scene = new GameScene();
            scene.AddSystem<RenderSystem>();
            scene.AddSystem<UpdateSystem>();
            scene.BackgroundColor = DefinedColors.MacabresoftPurple;

            var gridComponent = scene.AddChild().AddComponent<GridDrawerComponent>();
            gridComponent.Color = DefinedColors.MacabresoftBone * 0.5f;
            gridComponent.UseDynamicLineThickness = false;
            gridComponent.Grid = new TileGrid(Vector2.One);
            gridComponent.RenderOrder = -1;

            var skull = new SpriteSheet();
            this.Game.Project.Assets.SetContentMapping(skull.ContentId, "skull");
            this.Game.Project.Assets.AddAsset(skull);

            var skullEntity = scene.AddChild();
            skullEntity.LocalPosition += new Vector2(0f, 0.5f);
            this._skullRenderer = skullEntity.AddComponent<SpriteRenderComponent>();
            this._skullRenderer.SpriteReference.AssetId = skull.AssetId;
            this._skullRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            skullEntity.AddComponent<SampleInputComponent>();

            var leageMono = new Font();
            this.Game.Project.Assets.SetContentMapping(leageMono.ContentId, "League Mono");
            this.Game.Project.Assets.AddAsset(leageMono);

            var textRenderEntity = scene.AddChild();
            this._displayTextRenderer = textRenderEntity.AddComponent<TextRenderComponent>();
            this._displayTextRenderer.FontReference.AssetId = leageMono.AssetId;
            this._displayTextRenderer.Color = DefinedColors.MacabresoftYellow;
            this._displayTextRenderer.Text = this.DisplayText;
            this._displayTextRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            textRenderEntity.LocalScale = new Vector2(0.25f);
            textRenderEntity.LocalPosition = new Vector2(0f, -3f);

            var cameraEntity = scene.AddChild();
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

            this.Game.LoadScene(scene);

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