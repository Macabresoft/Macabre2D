namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;

    public class SampleAvaloniaGame : AvaloniaGame {
        protected override IAssetManager CreateSceneLevelAssetManager() {
            return this.Assets;
        }
    }
}