namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Linq;

    public sealed class MouseClickDebugger : BaseDrawer, IUpdateableComponent {
        private Camera _camera;

        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(this.LocalPosition - new Vector2(1f, 1f), this.LocalPosition + new Vector2(1f, 1f));
            }
        }

        public override void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            var spriteBatch = this._scene.Game.SpriteBatch;
            this.PrimitiveDrawer.DrawCircle(spriteBatch, 1f, this.WorldTransform.Position, 50, this.Color, 3f);
        }

        public void Update(GameTime gameTime) {
            var mouseState = Mouse.GetState();
            this.SetWorldPosition(this._camera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position));
            Console.WriteLine(this._camera.LocalPosition);
        }

        protected override void Initialize() {
            base.Initialize();
            this.Color = Color.Green;
            this.LineThickness = 3f;
            this._camera = this._scene.GetAllComponentsOfType<Camera>().FirstOrDefault();
        }
    }
}