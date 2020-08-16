namespace Macabresoft.MonoGame.Samples.Content {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework.Input;

    public sealed class VolumeController : BaseComponent, IUpdateableComponent {
        private AudioPlayerComponent _audioPlayer;

        public void Update(FrameTime frameTime, InputState inputState) {
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

        protected override void Initialize() {
            this._audioPlayer = this.Parent as AudioPlayerComponent;
        }
    }
}