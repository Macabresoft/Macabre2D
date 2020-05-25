namespace Macabre2D.Examples.PhysicsTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class VelocityChanger : BaseComponent, IUpdateableComponent {
        private IDynamicPhysicsBody _body;
        private float _speed = 0.5f;

        public void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.W)) {
                this._body.Velocity += new Vector2(0f, this._speed);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.S)) {
                this._body.Velocity += new Vector2(0f, -this._speed);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.A)) {
                this._body.Velocity += new Vector2(-this._speed, 0f);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D)) {
                this._body.Velocity += new Vector2(this._speed, 0f);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.Space)) {
                this._body.Velocity = Vector2.Zero;
            }
        }

        protected override void Initialize() {
            this._body = this.Parent as IDynamicPhysicsBody;
        }
    }
}