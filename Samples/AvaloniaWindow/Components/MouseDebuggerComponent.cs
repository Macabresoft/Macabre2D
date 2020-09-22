using Macabresoft.MonoGame.Core;
using Microsoft.Xna.Framework;

namespace Macabresoft.MonoGame.Samples.AvaloniaWindow.Components {

    public sealed class MouseDebuggerComponent : GameUpdateableComponent {
        private CameraComponent _cameraComponent;
        private TextRenderComponent _textRenderer;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Entity.TryGetComponent(out this._textRenderer);
            this.Entity.Parent.TryGetComponent(out this._cameraComponent);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this._textRenderer != null && this._cameraComponent != null) {
                var worldPosition = this._cameraComponent.ConvertPointFromScreenSpaceToWorldSpace(new Point(inputState.CurrentMouseState.X, inputState.CurrentMouseState.Y));
                this._textRenderer.Text = $"Mouse Position: ({worldPosition.X:F2}, {worldPosition.Y:F2})";
            }
        }
    }
}