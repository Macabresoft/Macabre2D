namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;

/// <summary>
/// Interface for an actor, which is
/// </summary>
public interface IActor {
    /// <summary>
    /// Gets the acceleration of this actor. This is the rate at which it gains speed when intentionally moving in a direction.
    /// </summary>
    float Acceleration => 6f;

    /// <summary>
    /// Gets a multiplier for air acceleration. By making this value less than one, the player will have less control in the air than they do when grounded.
    /// </summary>
    float AirAccelerationMultiplier => 0.9f;

    /// <summary>
    /// Gets the current state of this actor.
    /// </summary>
    ActorState CurrentState { get; }

    /// <summary>
    /// Gets the rate at which this actor decelerates in units per second.
    /// </summary>
    [DataMember]
    float DecelerationRate => 25f;

    /// <summary>
    /// Gets the gravity for this actor (or alternatively, the rate at which it will accelerate downwards when falling in units per second).
    /// </summary>
    [DataMember]
    float Gravity => 39f;

    /// <summary>
    /// Gets the initial velocity of a jump for this actor.
    /// </summary>
    float JumpVelocity => 8f;

    /// <summary>
    /// Gets the maximum horizontal velocity allowed.
    /// </summary>
    float MaximumHorizontalVelocity => 7f;

    /// <summary>
    /// Gets the minimum velocity before this actor will stop moving (when no force is causing it to move).
    /// </summary>
    float MinimumVelocity => 2f;

    /// <summary>
    /// Gets the current movement direction.
    /// </summary>
    HorizontalDirection MovementDirection { get; }

    /// <summary>
    /// Gets the maximum downward velocity when falling.
    /// </summary>
    float TerminalVelocity => 15f;

    /// <summary>
    /// Gets the horizontal acceleration of this actor.
    /// </summary>
    /// <returns>The horizontal acceleration.</returns>
    float GetHorizontalAcceleration() {
        return this.CurrentState.MovementKind == MovementKind.Moving ? this.Acceleration : this.Acceleration * this.AirAccelerationMultiplier;
    }
}