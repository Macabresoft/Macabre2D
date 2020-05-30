namespace Macabre2D.Examples.SynthTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class SongPlayerComponent : BaseComponent, IUpdateableComponent {
        private readonly SongPlayer _songPlayer;
        private bool _isPlaying = false;

        public SongPlayerComponent(Song song) {
            this._songPlayer = new SongPlayer(song);
        }

        public void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentKeyboardState.IsAnyKeyDown(Keys.Space) && !inputState.PreviousKeyboardState.IsKeyDown(Keys.Space)) {
                this._isPlaying = !this._isPlaying;

                if (!this._isPlaying) {
                    this._songPlayer.Stop();
                }
                else {
                    this._songPlayer.Play();
                }
            }

            if (this._isPlaying) {
                this._songPlayer.Buffer(0.8f, 1000);
            }
        }

        protected override void Initialize() {
            return;
        }
    }
}