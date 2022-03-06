namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public sealed class PlatformerPlayer : PlatformerActor {
    private readonly InputManager _input = new();
    private float _elapsedJumpSeconds;
    private float _elapsedRunSeconds;
    private float _jumpHoldTime = 0.1f;
    private float _timeUntilRun = 1f;

    /// <summary>
    /// Gets the acceleration in velocity per second when the player is changing directions.
    /// </summary>
    [DataMember]
    [Category("Movement")]
    public float AccelerationWhenChangingDirection { get; private set; } = 32f;

    /// <summary>
    /// Gets the deceleration when the player has no horizontal input.
    /// </summary>
    /// <remarks>
    /// This is how much the velocity will decrease per second. A higher value will feel snappier, while a lower value will feel slippery.
    /// </remarks>
    [DataMember]
    [Category("Movement")]
    public float Deceleration { get; private set; } = 24f;

    /// <summary>
    /// Gets the maximum time a jump can be held in seconds.
    /// </summary>
    [DataMember]
    [Category("Movement")]
    public float JumpHoldTime {
        get => this._jumpHoldTime;
        private set => this._jumpHoldTime = Math.Max(0f, value);
    }

    /// <summary>
    /// Gets the velocity when running.
    /// </summary>
    [DataMember]
    [Category("Movement")]
    public float RunVelocity { get; private set; }

    /// <summary>
    /// Gets the time until a walk becomes a run in seconds.
    /// </summary>
    [DataMember]
    [Category("Movement")]
    public float TimeUntilRun {
        get => this._timeUntilRun;
        private set => this._timeUntilRun = Math.Max(0f, value);
    }

    private float ElapsedRunSeconds {
        get => this._elapsedRunSeconds;
        set => this._elapsedRunSeconds = Math.Clamp(value, 0f, this.TimeUntilRun);
    }

    public override void Update(FrameTime frameTime, InputState inputState) {
        this._input.Update(inputState);
        base.Update(frameTime, inputState);
    }

    /// <inheritdoc />
    protected override ActorState HandleFalling(FrameTime frameTime, float anchorOffset) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        var movementKind = this.CurrentState.MovementKind;

        if (verticalVelocity < 0f && this.CheckIfHitGround(frameTime, verticalVelocity, anchorOffset, out var hit)) {
            this.SetWorldPosition(new Vector2(this.Transform.Position.X, hit.ContactPoint.Y + this.HalfSize.Y));
            movementKind = MovementKind.Idle;
            verticalVelocity = 0f;
        }
        else {
            if (verticalVelocity > 0f && this.CheckIfHitCeiling(frameTime, verticalVelocity, anchorOffset)) {
                verticalVelocity = 0f;
            }

            verticalVelocity += this.PhysicsSystem.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }

        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);
        if (movementKind == MovementKind.Idle && horizontalVelocity != 0f) {
            movementKind = MovementKind.Moving;
        }

        return new ActorState(movementKind, movementDirection, this.Transform.Position, new Vector2(horizontalVelocity, verticalVelocity));
    }

    /// <inheritdoc />
    protected override ActorState HandleIdle(FrameTime frameTime, float anchorOffset) {
        // TODO: should check if the user is suddenly falling due to a wall pushing them, a platform falling, or an elevator rising etc
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);

        if (this._input.JumpState == ButtonState.Pressed) {
            return this.GetJumpingState(horizontalVelocity, movementDirection);
        }

        if (!this.CheckIfStillGrounded(anchorOffset, out _)) {
            return this.GetFallingState(frameTime, horizontalVelocity, movementDirection);
        }

        return horizontalVelocity != 0f ? new ActorState(MovementKind.Moving, movementDirection, this.CurrentState.Position, new Vector2(horizontalVelocity, 0f)) : this.CurrentState;
    }

    /// <inheritdoc />
    protected override ActorState HandleJumping(FrameTime frameTime, float anchorOffset) {
        var verticalVelocity = this.JumpVelocity;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);
        var movementKind = this.CurrentState.MovementKind;

        if (this.CheckIfHitCeiling(frameTime, verticalVelocity, anchorOffset)) {
            verticalVelocity = 0f;
            movementKind = MovementKind.Falling;
            this._elapsedJumpSeconds = 0f;
        }
        else if (this._input.JumpState != ButtonState.Held || this._elapsedJumpSeconds > this.JumpHoldTime) {
            movementKind = MovementKind.Falling;
            this._elapsedJumpSeconds = 0f;
        }

        this._elapsedJumpSeconds += (float)frameTime.SecondsPassed;
        return new ActorState(movementKind, movementDirection, this.Transform.Position, new Vector2(horizontalVelocity, verticalVelocity));
    }

    /// <inheritdoc />
    protected override ActorState HandleMoving(FrameTime frameTime, float anchorOffset) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);
        var movementKind = this.CurrentState.MovementKind;

        if (!this.CheckIfStillGrounded(anchorOffset, out _)) {
            return this.GetFallingState(frameTime, horizontalVelocity, movementDirection);
        }

        if (this._input.JumpState == ButtonState.Pressed) {
            return this.GetJumpingState(horizontalVelocity, movementDirection);
        }

        return horizontalVelocity == 0f ? new ActorState(MovementKind.Idle, movementDirection, this.Transform.Position, Vector2.Zero) : new ActorState(movementKind, movementDirection, this.Transform.Position, new Vector2(horizontalVelocity, 0f));
    }

    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime, float anchorOffset) {
        var horizontalVelocity = this._input.HorizontalAxis * (this.ElapsedRunSeconds < this.TimeUntilRun ? this.MaximumHorizontalVelocity : this.RunVelocity);

        if (this.ElapsedRunSeconds > 0f) {
            horizontalVelocity = this._input.HorizontalAxis switch {
                < 0f when this.PreviousState.Velocity.X > 0f => Math.Max(this.PreviousState.Velocity.X - (float)frameTime.SecondsPassed * this.AccelerationWhenChangingDirection, -this.RunVelocity),
                > 0f when this.PreviousState.Velocity.X < 0f => Math.Min(this.PreviousState.Velocity.X + (float)frameTime.SecondsPassed * this.AccelerationWhenChangingDirection, this.RunVelocity),
                _ => horizontalVelocity
            };
        }

        var movingDirection = this._input.HorizontalAxis switch {
            > 0f => HorizontalDirection.Right,
            < 0f => HorizontalDirection.Left,
            _ => this.CurrentState.FacingDirection
        };

        if (horizontalVelocity != 0f) {
            if (this.CheckIfHitWall(frameTime, horizontalVelocity, true, anchorOffset)) {
                horizontalVelocity = 0f;
            }

            this.CheckIfHitWall(frameTime, -horizontalVelocity, false, anchorOffset);
        }
        else {
            this.CheckIfHitWall(frameTime, 0f, false, anchorOffset);
        }

        if (horizontalVelocity != 0f) {
            this.ElapsedRunSeconds += (float)frameTime.SecondsPassed;
        }
        else if (this.ElapsedRunSeconds > 0f) {
            horizontalVelocity = this.PreviousState.Velocity.X switch {
                > 0f => (float)Math.Max(0f, this.PreviousState.Velocity.X - frameTime.SecondsPassed * this.Deceleration),
                < 0f => (float)Math.Min(0f, this.PreviousState.Velocity.X + frameTime.SecondsPassed * this.Deceleration),
                _ => horizontalVelocity
            };

            this.ElapsedRunSeconds -= (float)frameTime.SecondsPassed * 2f;
        }

        return (horizontalVelocity, movingDirection);
    }
}