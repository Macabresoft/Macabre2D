namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public class Scaler : BaseComponent, IUpdateableComponent {
        private float _currentSign = 1f;

        public void Update(GameTime gameTime) {
            if ((this.LocalScale.X > 2f && this._currentSign > 0) || (this.LocalScale.X < -2f && this._currentSign < 0)) {
                this._currentSign *= -1f;
            }

            this.LocalScale += Vector2.One * gameTime.ElapsedGameTime.Milliseconds * 0.001f * this._currentSign;
        }

        protected override void Initialize() {
            return;
        }
    }
}