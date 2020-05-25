namespace Macabre2D.Examples.MultiPlatformTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class MovingDot : BaseComponent, IUpdateableComponent {
        private readonly float _speed = 1f;

        public void Update(FrameTime frameTime, InputState inputState) {
            if (this.Parent == null) {
                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.W)) {
                    this.LocalPosition += new Vector2(0f, this._speed);
                }

                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.S)) {
                    this.LocalPosition += new Vector2(0f, -this._speed);
                }

                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.A)) {
                    this.LocalPosition += new Vector2(-this._speed, 0f);
                }

                if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D)) {
                    this.LocalPosition += new Vector2(this._speed, 0f);
                }
            }
        }

        protected override void Initialize() {
            return;
        }
    }
}