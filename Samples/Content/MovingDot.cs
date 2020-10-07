namespace Macabresoft.MonoGame.Samples.Content {

    using Macabresoft.MonoGame.Core2D;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class MovingDot : GameUpdateableComponent {
        private readonly float _speed = 1f;

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.W)) {
                this.Entity.LocalPosition += new Vector2(0f, this._speed);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.S)) {
                this.Entity.LocalPosition += new Vector2(0f, -this._speed);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.A)) {
                this.Entity.LocalPosition += new Vector2(-this._speed, 0f);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D)) {
                this.Entity.LocalPosition += new Vector2(this._speed, 0f);
            }
        }
    }
}