namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow;

using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Samples.AvaloniaWindow.Entities;
using Macabresoft.Macabre2D.UI.AvaloniaInterop;
using Macabresoft.Macabre2D.UI.Common;
using Microsoft.Xna.Framework;
using ReactiveUI;

public sealed class SkullViewModel : BaseViewModel {
    private Camera _camera;
    private string _displayText = @"github.com/Macabresoft/Macabresoft.Macabre2D";
    private TextRenderer _displayTextRenderer;
    private SpriteRenderer _skullRenderer;

    public SkullViewModel(IAvaloniaGame game) : base() {
        game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        this.Scene = this.CreateScene(game);
    }

    public IScene Scene { get; }

    public string DisplayText {
        get => this._displayText;

        set {
            this.RaiseAndSetIfChanged(ref this._displayText, value);
            if (this._displayTextRenderer != null) {
                this._displayTextRenderer.Text = value;
            }
        }
    }

    private IScene CreateScene(IAvaloniaGame game) {
        var scene = new Scene();
        scene.AddSystem<RenderSystem>();
        scene.AddSystem<UpdateSystem>();
        scene.BackgroundColor = DefinedColors.MacabresoftPurple;

        var skull = new SpriteSheet();

        this._skullRenderer = scene.AddChild<SpriteRenderer>();
        this._skullRenderer.LocalPosition += new Vector2(0f, 0.5f);
        this._skullRenderer.SpriteReference.Initialize(skull);
        this._skullRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
        this._skullRenderer.AddChild<SampleInputEntity>();

        var leagueMono = new Font();

        this._displayTextRenderer = scene.AddChild<TextRenderer>();
        this._displayTextRenderer.FontReference.Initialize(leagueMono);
        this._displayTextRenderer.Color = DefinedColors.MacabresoftYellow;
        this._displayTextRenderer.Text = this.DisplayText;
        this._displayTextRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
        this._displayTextRenderer.LocalScale = new Vector2(0.25f);
        this._displayTextRenderer.LocalPosition = new Vector2(0f, -3f);

        this._camera = scene.AddChild<Camera>();
        this._camera.ViewHeight = 9f;
        var cameraTextRenderer = this._camera.AddChild<TextRenderer>();
        cameraTextRenderer.FontReference.Initialize(leagueMono);
        cameraTextRenderer.Color = DefinedColors.MacabresoftBone;
        cameraTextRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
        cameraTextRenderer.Text = "Mouse Position: (0.00, 0.00)";
        cameraTextRenderer.AddChild<MouseDebuggerEntity>();
        cameraTextRenderer.LocalScale = new Vector2(0.1f);
        cameraTextRenderer.LocalPosition = new Vector2(0f, -2.5f);
        var frameRateDisplay = this._camera.AddChild<FrameRateDisplayEntity>();
        frameRateDisplay.FontReference.Initialize(leagueMono);
        frameRateDisplay.Color = DefinedColors.ZvukostiGreen;
        frameRateDisplay.LocalScale = new Vector2(0.1f);

        game.Assets.RegisterMetadata(new ContentMetadata(skull, new[] { "skull" }, ".png"));
        game.Assets.RegisterMetadata(new ContentMetadata(leagueMono, new[] { "League Mono" }, ".spritefont"));
        return scene;
    }

    private void Game_ViewportSizeChanged(object sender, Point e) {
        this.ResetCamera();
    }

    private void ResetCamera() {
        if (this._camera != null) {
            // This probably seems weird, but it resets the view height which causes the view
            // matrix and bounding area to be reevaluated.
            this._camera.ViewHeight += 1;
            this._camera.ViewHeight -= 1;
        }
    }
}