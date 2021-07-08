namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow.Entities {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public sealed class MouseDebuggerEntity : UpdateableEntity {
        private Camera _camera;
        private TextRenderer _textRenderer;

        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);
            this.TryGetParentEntity(out this._textRenderer);
            this.TryGetParentEntity(out this._camera);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this._textRenderer != null && this._camera != null) {
                var worldPosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(new Point(inputState.CurrentMouseState.X, inputState.CurrentMouseState.Y));
                this._textRenderer.Text = $"Mouse Position: ({worldPosition.X:F2}, {worldPosition.Y:F2})";
            }
        }
    }
}