namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class MovingDot : BaseComponent, IUpdateableComponent {
        private readonly float _speed = 1f;

        public void Update(GameTime gameTime) {
            if (this.Parent == null) {
                var keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.W)) {
                    this.LocalPosition += new Vector2(0f, this._speed);
                }

                if (keyboardState.IsKeyDown(Keys.S)) {
                    this.LocalPosition += new Vector2(0f, -this._speed);
                }

                if (keyboardState.IsKeyDown(Keys.A)) {
                    this.LocalPosition += new Vector2(-this._speed, 0f);
                }

                if (keyboardState.IsKeyDown(Keys.D)) {
                    this.LocalPosition += new Vector2(this._speed, 0f);
                }
            }
        }

        protected override void Initialize() {
            return;
        }
    }
}