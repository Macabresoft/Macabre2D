namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public sealed class PlayerPlatformerActor : PlatformerActor {
    private readonly InputManager _input = new();
    private float _elapsedJumpSeconds;

    /// <summary>
    /// Gets the maximum time a jump can be held in seconds.
    /// </summary>
    [DataMember]
    public float JumpHoldTime { get; private set; } = 0.1f;

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
        this._elapsedJumpSeconds += (float)frameTime.SecondsPassed;
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

    // TODO: need to make this generic for gamepads/keyboards/user settings
    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime, float anchorOffset) {
        var horizontalVelocity = this._input.HorizontalAxis * this.MaximumHorizontalVelocity;

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

        return (horizontalVelocity, movingDirection);
    }
}