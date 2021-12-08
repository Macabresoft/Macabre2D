namespace Macabresoft.Macabre2D.Samples.Content;

using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

public sealed class VolumeController : UpdateableEntity {
    private AudioPlayer _audioPlayer;

    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);
        this.TryGetParentEntity(out this._audioPlayer);
    }

    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this._audioPlayer != null) {
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