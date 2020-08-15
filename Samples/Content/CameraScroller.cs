namespace Macabre2D.Samples.Content {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class CameraScroller : BaseComponent, IUpdateableComponent {

        [Child]
        private Camera _camera;

        private int _previousScrollValue;

        public void Update(FrameTime frameTime, InputState inputState) {
            if (this._camera != null) {
                if (inputState.CurrentMouseState.ScrollWheelValue != this._previousScrollValue) {
                    var scrollViewChange = (float)(frameTime.SecondsPassed * (this._previousScrollValue - inputState.CurrentMouseState.ScrollWheelValue)) * 2f;
                    var isZoomIn = scrollViewChange < 0;
                    if (isZoomIn) {
                        this._camera.ZoomTo(inputState.CurrentMouseState.Position, scrollViewChange * -1f);
                    }
                    else {
                        this._camera.ViewHeight += scrollViewChange;
                    }
                    this._previousScrollValue = inputState.CurrentMouseState.ScrollWheelValue;
                }

                var movementMultiplier = (float)frameTime.SecondsPassed * this._camera.ViewHeight;

                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.W)) {
                    this._camera.LocalPosition += new Vector2(0f, movementMultiplier);
                }

                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.A)) {
                    this._camera.LocalPosition += new Vector2(movementMultiplier * -1f, 0f);
                }

                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.S)) {
                    this._camera.LocalPosition += new Vector2(0f, movementMultiplier * -1f);
                }

                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D)) {
                    this._camera.LocalPosition += new Vector2(movementMultiplier, 0f);
                }
            }
        }

        protected override void Initialize() {
            return;
        }
    }
}