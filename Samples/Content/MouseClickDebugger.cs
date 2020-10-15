namespace Macabresoft.Macabre2D.Samples.Content {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public sealed class MouseClickDebugger : BaseDrawerComponent, IGameUpdateableComponent {
        private CameraComponent _camera;

        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(this.Entity.LocalPosition - new Vector2(1f, 1f), this.Entity.LocalPosition + new Vector2(1f, 1f));
            }
        }

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Color = Color.Green;
            this.LineThickness = 3f;
            this.Entity.Parent.TryGetComponent(out this._camera);
        }

        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            this.PrimitiveDrawer?.DrawCircle(this.Entity.Scene.Game.SpriteBatch, 1f, this.Entity.Transform.Position, 50, this.Color, 3f);
        }

        public void Update(FrameTime frameTime, InputState inputState) {
            this.Entity.SetWorldPosition(this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position));
        }
    }
}