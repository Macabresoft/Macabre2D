namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow.Components {

    using Macabresoft.Macabre2D.Framework;

    public sealed class FrameRateDisplayComponentOLD : GameUpdateableComponent {
        private FrameRateComponent _frameRateComponent;
        private TextRenderComponent _textRenderComponent;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            this.Entity.TryGetComponent(out this._frameRateComponent);
            this.Entity.TryGetComponent(out this._textRenderComponent);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            this._textRenderComponent.Text = $"FPS: {this._frameRateComponent.AverageFrameRate:F2}";
        }
    }
}