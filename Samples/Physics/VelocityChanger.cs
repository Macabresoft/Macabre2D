namespace Macabresoft.MonoGame.Samples.Physics {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class VelocityChanger : GameUpdateableComponent {
        private IDynamicPhysicsBody? _body;
        private float _speed = 0.5f;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (this.Entity.TryGetComponent<IDynamicPhysicsBody>(out var body) && body != null) {
                this._body = body;
            }
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
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
    }
}