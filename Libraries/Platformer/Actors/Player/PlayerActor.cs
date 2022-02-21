namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public class PlayerPlatformerActor : PlatformerActor {
    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.ActorSize.X > 0f && this.ActorSize.Y > 0f) {
            // TODO: check user input and collisions, set the state, then call base.Update(...)
        }

        base.Update(frameTime, inputState);
    }
}