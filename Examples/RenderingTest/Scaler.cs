namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public class Scaler : BaseComponent, IUpdateableComponent {
        private float _currentSign = 1f;

#pragma warning disable CS0649

        [Child]
        private SpriteRenderComponent _spriteRenderer;

#pragma warning disable CS0649

        public void Update(FrameTime frameTime) {
            if ((this.LocalScale.X > 2f && this._currentSign > 0) || (this.LocalScale.X < -2f && this._currentSign < 0)) {
                this._currentSign *= -1f;
            }

            this.LocalScale += Vector2.One * (float)frameTime.MillisecondsPassed * 0.001f * this._currentSign;
            this._spriteRenderer.Rotation += (float)frameTime.MillisecondsPassed * 0.003f;
        }

        protected override void Initialize() {
            return;
        }
    }
}