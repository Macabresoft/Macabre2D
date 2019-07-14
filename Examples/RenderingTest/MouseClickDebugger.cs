namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Linq;

    public sealed class MouseClickDebugger : BaseDrawer, IUpdateableComponent {
        private Camera _camera;

        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(this.LocalPosition - new Vector2(1f, 1f), this.LocalPosition + new Vector2(1f, 1f));
            }
        }

        public override void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            var spriteBatch = MacabreGame.Instance.SpriteBatch;
            this.PrimitiveDrawer.DrawCircle(spriteBatch, 1f, this.WorldTransform.Position, 50, this.Color, 3f);
        }

        public void Update(GameTime gameTime) {
            var mouseState = Mouse.GetState();
            this.SetWorldPosition(this._camera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position));
        }

        protected override void Initialize() {
            base.Initialize();
            this.Color = Color.Green;
            this.LineThickness = 3f;
            this._camera = this.Scene.GetAllComponentsOfType<Camera>().FirstOrDefault();
        }
    }
}