namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class CameraScroller : BaseComponent, IUpdateableComponent {
#pragma warning disable CS0649

        [Child]
        private Camera _camera;

#pragma warning restore CS0649
        private int _previousScrollValue;

        public void Update(GameTime gameTime) {
            if (this._camera != null) {
                var mouseState = Mouse.GetState();
                if (mouseState.ScrollWheelValue != this._previousScrollValue) {
                    var scrollViewChange = (float)(gameTime.ElapsedGameTime.TotalSeconds * (this._previousScrollValue - mouseState.ScrollWheelValue)) * 2f;
                    var isZoomIn = scrollViewChange < 0;
                    if (isZoomIn) {
                        this._camera.ZoomTo(mouseState.Position, scrollViewChange * -1f);
                    }
                    else {
                        this._camera.ViewHeight += scrollViewChange;
                    }
                    this._previousScrollValue = mouseState.ScrollWheelValue;
                }

                var keyboardState = Keyboard.GetState();
                var movementMultiplier = (float)gameTime.ElapsedGameTime.TotalSeconds * this._camera.ViewHeight;

                if (keyboardState.IsKeyDown(Keys.W)) {
                    this._camera.LocalPosition += new Vector2(0f, movementMultiplier);
                }

                if (keyboardState.IsKeyDown(Keys.A)) {
                    this._camera.LocalPosition += new Vector2(movementMultiplier * -1f, 0f);
                }

                if (keyboardState.IsKeyDown(Keys.S)) {
                    this._camera.LocalPosition += new Vector2(0f, movementMultiplier * -1f);
                }

                if (keyboardState.IsKeyDown(Keys.D)) {
                    this._camera.LocalPosition += new Vector2(movementMultiplier, 0f);
                }
            }
        }

        protected override void Initialize() {
            return;
        }
    }
}