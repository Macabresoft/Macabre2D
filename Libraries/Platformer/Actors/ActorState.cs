namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Microsoft.Xna.Framework;

/// <summary>
/// Describes the actor's state for a single frame.
/// </summary>
public readonly struct ActorState {
    /// <summary>
    /// The default actor state.
    /// </summary>
    public static readonly ActorState Default = default;

    /// <summary>
    /// The world position.
    /// </summary>
    public readonly Vector2 Position;

    /// <summary>
    /// The type of state.
    /// </summary>
    public readonly StateType StateType;

    /// <summary>
    /// The direction the actor was facing.
    /// </summary>
    public readonly HorizontalDirection FacingDirection = HorizontalDirection.Right;

    /// <summary>
    /// The velocity.
    /// </summary>
    public readonly Vector2 Velocity;

    /// <summary>
    /// The number of seconds the actor has been in this state.
    /// </summary>
    public readonly float SecondsInState;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorState" /> class.
    /// </summary>
    /// <param name="stateType">The state type.</param>
    /// <param name="facingDirection">The direction the actor is facing.</param>
    /// <param name="position">The position.</param>
    /// <param name="velocity">The velocity.</param>
    /// <param name="secondsInState">The seconds in the same state.</param>
    public ActorState(StateType stateType, HorizontalDirection facingDirection, Vector2 position, Vector2 velocity, float secondsInState) {
        this.StateType = stateType;
        this.FacingDirection = facingDirection;
        this.Position = position;
        this.Velocity = velocity;
        this.SecondsInState = Math.Max(0f, secondsInState);
    }

    /// <inheritdoc cref="object" />
    public static bool operator !=(ActorState left, ActorState right) {
        return !(left == right);
    }

    /// <inheritdoc cref="object" />
    public static bool operator ==(ActorState left, ActorState right) {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public override bool Equals(object? other) {
        return other is ActorState playerState && this.Equals(playerState);
    }

    private bool Equals(ActorState other) {
        return this.StateType.Equals(other.StateType) &&
            this.FacingDirection.Equals(other.FacingDirection) &&
               this.SecondsInState.Equals(other.SecondsInState) &&
               this.Position.Equals(other.Position) &&
               this.Velocity.Equals(other.Velocity);
    }

    /// <inheritdoc />
    public override int GetHashCode() {
        return HashCode.Combine(this.Position, (int)this.FacingDirection, this.Velocity, this.SecondsInState);
    }
}