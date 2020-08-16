namespace Macabresoft.MonoGame.Samples.Content {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;

    public sealed class MouseClickDebugger : BaseDrawerComponent, IUpdateableComponent {
        private Camera _camera;

        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(this.LocalPosition - new Vector2(1f, 1f), this.LocalPosition + new Vector2(1f, 1f));
            }
        }

        public override void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            var spriteBatch = MacabreGame.Instance.SpriteBatch;
            this.PrimitiveDrawer?.DrawCircle(spriteBatch, 1f, this.WorldTransform.Position, 50, this.Color, 3f);
        }

        public void Update(FrameTime frameTime, InputState inputState) {
            this.SetWorldPosition(this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position));
        }

        protected override void Initialize() {
            base.Initialize();
            this.Color = Color.Green;
            this.LineThickness = 3f;
            this._camera = this.Scene.FindComponentOfType<Camera>();
        }
    }
}