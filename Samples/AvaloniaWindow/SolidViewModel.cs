namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow;

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Microsoft.Xna.Framework;

public sealed class SolidViewModel : BaseViewModel {
    private readonly ICamera _camera;

    public SolidViewModel(IGame game) : base() {
        game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        this.Scene = this.CreateScene(out this._camera);
    }

    public IScene Scene { get; }

    public IScene CreateScene(out ICamera camera) {
        var scene = new Scene();
        scene.AddSystem<RenderSystem>();
        scene.AddSystem<UpdateSystem>();
        scene.BackgroundColor = DefinedColors.CosmicJam;

        camera = scene.AddChild<Camera>();
        camera.ViewHeight = 6f;
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