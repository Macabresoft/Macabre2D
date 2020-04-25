namespace Macabre2D.Examples.MultiPlatformTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class VolumeController : BaseComponent, IUpdateableComponent {
        private AudioPlayer _audioClip;

        public void Update(FrameTime frameTime) {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W)) {
                this._audioClip.Volume += 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.S)) {
                this._audioClip.Volume -= 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.A)) {
                this._audioClip.Pitch += 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.D)) {
                this._audioClip.Pitch -= 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.Space)) {
                this._audioClip.Play();
            }
        }

        protected override void Initialize() {
            this._audioClip = this.Parent as AudioPlayer;
        }
    }
}