namespace Macabresoft.Macabre2D.Samples.Content {
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class MovingDot : GameUpdateableEntity {
        private readonly float _speed = 1f;

        public override void Update(FrameTime frameTime, InputState inputState) {
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
}