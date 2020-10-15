namespace Macabresoft.Macabre2D.Samples.Content {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class VolumeController : GameUpdateableComponent {
        private AudioPlayerComponent _audioPlayer;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Entity.TryGetComponent(out this._audioPlayer);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.W)) {
                this._audioPlayer.Volume += 0.1f;
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.S)) {
                this._audioPlayer.Volume -= 0.1f;
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.A)) {
                this._audioPlayer.Pitch += 0.1f;
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D)) {
                this._audioPlayer.Pitch -= 0.1f;
            }

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.Space)) {
                this._audioPlayer.Play();
            }
        }
    }
}