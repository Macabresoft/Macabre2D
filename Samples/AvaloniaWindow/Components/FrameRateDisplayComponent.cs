﻿namespace Macabresoft.MonoGame.Samples.AvaloniaWindow.Components {

    using Macabresoft.MonoGame.Core;

    public sealed class FrameRateDisplayComponent : GameUpdateableComponent {
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