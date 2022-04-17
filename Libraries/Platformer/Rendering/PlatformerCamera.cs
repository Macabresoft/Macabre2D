namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private float _lerpSpeed = 5f;

    /// <summary>
    /// Gets or sets the distance the followed object is allowed to go above before following.
    /// </summary>
    [DataMember(Name = "Above (Distance Allowed)")]
    public float DistanceAllowedAbove { get; set; } = 6f;

    /// <summary>
    /// Gets or sets the distance the followed object is allowed to go below before following.
    /// </summary>
    [DataMember(Name = "Below (Distance Allowed)")]
    public float DistanceAllowedBelow { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the offset of how this camera follows the actor.
    /// </summary>
    [DataMember]
    public Vector2 FollowOffset { get; set; }

    /// <summary>
    /// Gets or sets the horizontal distance allowed from the player.
    /// </summary>
    [DataMember]
    public float HorizontalDistanceAllowed { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the lerp speed.
    /// </summary>
    [DataMember]
    public float LerpSpeed {
        get => this._lerpSpeed;
        set => this.Set(ref this._lerpSpeed, value);
    }

    /// <summary>
    /// Updates the position according to the specified actor state.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    /// <param name="currentState">The current state.</param>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="isOnPlatform">A value indicating whether or not the actor is on a platform.</param>
    public void UpdateDesiredPosition(ActorState previousState, ActorState currentState, FrameTime frameTime, bool isOnPlatform) {
        var x = this.LocalPosition.X;
        var offsetX = this.FollowOffset.X;
        if (this.HorizontalDistanceAllowed != 0f && this.TransformInheritance is TransformInheritance.Both or TransformInheritance.X) {
            if (isOnPlatform) {
                if (x != 0f) {
                    x = this.Lerp(this.LocalPosition.X, offsetX, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
            else {
                x = Math.Clamp(this.LocalPosition.X + currentState.Position.X - previousState.Position.X, -this.HorizontalDistanceAllowed + offsetX, this.HorizontalDistanceAllowed + offsetX);
            }
        }

        var y = this.LocalPosition.Y;
        var offsetY = this.FollowOffset.Y;
        var aboveDistance = -this.DistanceAllowedBelow + offsetY;
        var belowDistance = this.DistanceAllowedAbove + offsetY;
        if ((aboveDistance > 0f || belowDistance > 0f) && this.TransformInheritance is TransformInheritance.Both or TransformInheritance.Y) {
            if (isOnPlatform) {
                if (y != 0f) {
                    y = this.Lerp(this.LocalPosition.Y, offsetY, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
            else {
                var yMovement = currentState.Position.Y - previousState.Position.Y;
                if (Math.Abs(yMovement) > 0.001f) {
                    if (currentState.Velocity.Y < 0f && Math.Abs(currentState.Velocity.Y - previousState.Velocity.Y) < 0.001f) {
                        y = this.Lerp(this.LocalPosition.Y, aboveDistance, (float)frameTime.SecondsPassed);
                    }
                    else {
                        y = Math.Clamp(this.LocalPosition.Y + yMovement, aboveDistance, belowDistance);
                    }
                }
                else if (y != 0f) {
                    y = this.Lerp(this.LocalPosition.Y, offsetY, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
        }

        this.LocalPosition = new Vector2(x, y);
    }

    private float Lerp(float current, float desired, float amount) {
        var result = current + (desired - current) * amount;

        if (Math.Abs(result - desired) < this.Settings.InversePixelsPerUnit * 0.5f) {
            result = desired;
        }

        return result;
    }
}