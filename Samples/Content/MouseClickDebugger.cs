namespace Macabresoft.Macabre2D.Samples.Content {
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public sealed class MouseClickDebugger : BaseDrawer, IUpdateableEntity {
        private Camera _camera;

        public override BoundingArea BoundingArea => new(this.LocalPosition - new Vector2(1f, 1f), this.LocalPosition + new Vector2(1f, 1f));


        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);
            this.Color = Color.Green;
            this.LineThickness = 3f;
            this.TryGetParentEntity(out this._camera);
        }

        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                this.PrimitiveDrawer?.DrawCircle(
                    spriteBatch,
                    this.Scene.Game.Project.Settings.PixelsPerUnit,
                    1f,
                    this.Transform.Position,
                    50,
                    this.Color,
                    3f);
            }
        }

        public void Update(FrameTime frameTime, InputState inputState) {
            this.SetWorldPosition(this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position));
        }
    }
}