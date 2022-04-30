namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private const float FloatingPointTolerance = 0.001f;
    private bool _isFreeFalling;
    private float _lerpSpeed = 5f;

    /// <summary>
    /// Gets or sets the area in which the player can move without the camera adjusting its position.
    /// </summary>
    [DataMember]
    public BoundingArea FollowArea { get; set; }

    /// <summary>
    /// Gets or sets the offset of how this camera follows the actor.
    /// </summary>
    [DataMember]
    public Vector2 FollowOffset { get; set; }

    /// <summary>
    /// Gets or sets the lerp speed.
    /// </summary>
    [DataMember]
    public float LerpSpeed {
        get => this._lerpSpeed;
        set => this.Set(ref this._lerpSpeed, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.LocalPosition = this.FollowOffset;
    }

    /// <summary>
    /// Updates the position according to the specified actor state.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <param name="previousState">The previous state.</param>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="isOnPlatform">A value indicating whether or not the actor is on a platform.</param>
    public void UpdateDesiredPosition(ActorState currentState, ActorState previousState, FrameTime frameTime, bool isOnPlatform) {
        var x = this.GetXPosition(currentState, previousState, frameTime, isOnPlatform);
        var y = this.GetYPosition(currentState, previousState, frameTime, isOnPlatform);
        this.LocalPosition = new Vector2(x, y);
    }

    /// <summary>
    /// Gets the X position for this camera given the actor's state.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <param name="previousState">The previous state.</param>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="isOnPlatform">A value indicating whether or not the actor is on a platform.</param>
    /// <returns>The x position.</returns>
    protected virtual float GetXPosition(ActorState currentState, ActorState previousState, FrameTime frameTime, bool isOnPlatform) {
        var x = this.LocalPosition.X;
        var offsetX = this.FollowOffset.X;
        if (this.FollowArea.Width > 0f && this.TransformInheritance is TransformInheritance.Both or TransformInheritance.X) {
            if (isOnPlatform) {
                if (x != 0f) {
                    x = this.Lerp(this.LocalPosition.X, offsetX, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
            else {
                x = Math.Clamp(this.LocalPosition.X + previousState.Position.X - currentState.Position.X, this.FollowArea.Minimum.X + offsetX, this.FollowArea.Maximum.X + offsetX);
            }
        }

        return x;
    }

    /// <summary>
    /// Gets the Y position for this camera given the actor's state.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <param name="previousState">The previous state.</param>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="isOnPlatform">A value indicating whether or not the actor is on a platform.</param>
    /// <returns>The Y position.</returns>
    protected virtual float GetYPosition(ActorState currentState, ActorState previousState, FrameTime frameTime, bool isOnPlatform) {
        var y = this.LocalPosition.Y;
        var offsetY = this.FollowOffset.Y;
        var minimumY = -this.FollowArea.Maximum.Y + offsetY;
        var maximumY = -this.FollowArea.Minimum.Y + offsetY;
        if ((minimumY > 0f || maximumY > 0f) && this.TransformInheritance is TransformInheritance.Both or TransformInheritance.Y) {
            if (isOnPlatform) {
                y = this.Lerp(this.LocalPosition.Y, offsetY, (float)frameTime.SecondsPassed * this._lerpSpeed);
            }
            else {
                if (currentState.StateType is StateType.Falling or StateType.Jumping) {
                    var isFalling = previousState.Velocity.Y < 0f && Math.Abs(currentState.Velocity.Y - previousState.Velocity.Y) < FloatingPointTolerance;
                    if (this._isFreeFalling && isFalling) {
                        y = this.Lerp(this.LocalPosition.Y, minimumY, (float)frameTime.SecondsPassed);
                    }
                    else {
                        var yMovement = currentState.Position.Y - previousState.Position.Y;
                        y = Math.Clamp(this.LocalPosition.Y - yMovement, minimumY, maximumY);
                        this._isFreeFalling = Math.Abs(y - maximumY) < FloatingPointTolerance && isFalling;
                    }
                }
                else if (!this._isFreeFalling && previousState.StateType == StateType.Falling) {
                    var yMovement = currentState.Position.Y - previousState.Position.Y;
                    y = Math.Clamp(this.LocalPosition.Y - yMovement, minimumY, maximumY);
                }
                else {
                    this._isFreeFalling = false;
                    y = this.Lerp(this.LocalPosition.Y, offsetY, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
        }

        return y;
    }

    /// <summary>
    /// Lerps a float value.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="desired">The desired value.</param>
    /// <param name="amount">The amount to lerp (a value between 0 and 1)</param>
    /// <returns>The lerped value.</returns>
    protected float Lerp(float current, float desired, float amount) {
        var result = current + (desired - current) * amount;

        if (Math.Abs(result - desired) < this.Settings.InversePixelsPerUnit * 0.5f) {
            result = desired;
        }

        return result;
    }
}