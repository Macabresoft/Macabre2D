namespace Macabresoft.Macabre2D.Libraries.Platformer;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public class PlayerPlatformerActor : PlatformerActor {
    /// <inheritdoc />
    protected override ActorState GetNewActorState() {
        if (this.Size.X > 0f && this.Size.Y > 0f) {
            // TODO: check user input and collisions, set the state, then call base.Update(...)
        }

        return ActorState.Default;
    }
}