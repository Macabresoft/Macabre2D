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
    /// The kind of movement.
    /// </summary>
    public readonly MovementKind MovementKind;

    /// <summary>
    /// The velocity.
    /// </summary>
    public readonly Vector2 Velocity;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorState" /> class.
    /// </summary>
    /// <param name="movementKind">The kind of movement..</param>
    /// <param name="position">The position.</param>
    /// <param name="velocity">The velocity.</param>
    public ActorState(MovementKind movementKind, Vector2 position, Vector2 velocity) {
        this.MovementKind = movementKind;
        this.Position = position;
        this.Velocity = velocity;
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
        return this.Position.Equals(other.Position) &&
               this.MovementKind == other.MovementKind &&
               this.Velocity.Equals(other.Velocity);
    }

    /// <inheritdoc />
    public override int GetHashCode() {
        return HashCode.Combine(this.Position, (int)this.MovementKind, this.Velocity);
    }
}