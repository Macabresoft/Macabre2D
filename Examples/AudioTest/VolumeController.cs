using Macabre2D.Framework;
using Macabre2D.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Macabre2D.Examples.AudioTest {

    public sealed class VolumeController : BaseComponent, IUpdateableComponent {
        private AudioPlayer _audioClip;

        public void Update(GameTime gameTime) {
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