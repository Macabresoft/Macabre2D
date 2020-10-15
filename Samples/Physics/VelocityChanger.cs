namespace Macabresoft.Macabre2D.Samples.Physics {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class VelocityChanger : GameUpdateableComponent {

        public const float Speed = 0.5f;


        private IDynamicPhysicsBody _body;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (this.Entity.TryGetComponent<IDynamicPhysicsBody>(out var body) && body != null) {
                this._body = body;
            }
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.W)) {
                this._body.Velocity += new Vector2(0f, Speed);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.S)) {
                this._body.Velocity += new Vector2(0f, -Speed);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.A)) {
                this._body.Velocity += new Vector2(-Speed, 0f);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D)) {
                this._body.Velocity += new Vector2(Speed, 0f);
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.Space)) {
                this._body.Velocity = Vector2.Zero;
            }
        }
    }
}