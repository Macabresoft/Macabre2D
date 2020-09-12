namespace Macabresoft.MonoGame.Samples.Content {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;

    public class Scaler : GameUpdateableComponent {
        private float _currentSign = 1f;

        private SpriteRenderComponent _spriteRenderer;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Entity.TryGetComponent(out this._spriteRenderer);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if ((this.Entity.LocalScale.X > 2f && this._currentSign > 0) || (this.Entity.LocalScale.X < -2f && this._currentSign < 0)) {
                this._currentSign *= -1f;
            }

            this.Entity.LocalScale += Vector2.One * (float)frameTime.MillisecondsPassed * 0.001f * this._currentSign;
            this._spriteRenderer.Rotation += (float)frameTime.MillisecondsPassed * 0.003f;
        }
    }
}