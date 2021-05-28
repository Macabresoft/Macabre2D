namespace Macabresoft.Macabre2D.Samples.Content {
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public class Scaler : UpdateableEntity {
        private float _currentSign = 1f;

        private SpriteRenderer _spriteRenderer;

        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);
            this.TryGetParentEntity(out this._spriteRenderer);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this._spriteRenderer != null) {
                if (this._spriteRenderer.LocalScale.X > 2f && this._currentSign > 0 || this._spriteRenderer.LocalScale.X < -2f && this._currentSign < 0) {
                    this._currentSign *= -1f;
                }

                this._spriteRenderer.LocalScale += Vector2.One * (float)frameTime.MillisecondsPassed * 0.001f * this._currentSign;
                this._spriteRenderer.Rotation += (float)frameTime.MillisecondsPassed * 0.003f;
            }
        }
    }
}