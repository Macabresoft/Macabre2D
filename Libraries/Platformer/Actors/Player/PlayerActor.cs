namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public sealed class PlayerPlatformerActor : PlatformerActor {
    private float _elapsedJumpSeconds;

    /// <summary>
    /// Gets the maximum time a jump can be held in seconds.
    /// </summary>
    [DataMember]
    public float JumpHoldTime { get; private set; } = 0.1f;

    /// <inheritdoc />
    protected override ActorState HandleFalling(FrameTime frameTime, InputState inputState, float anchorOffset) {
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

        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, inputState, anchorOffset);
        this.MovementDirection = movementDirection;
        if (movementKind == MovementKind.Idle && horizontalVelocity != 0f) {
            movementKind = MovementKind.Moving;
        }

        return new ActorState(movementKind, movementDirection, this.Transform.Position, new Vector2(horizontalVelocity, verticalVelocity));
    }

    /// <inheritdoc />
    protected override ActorState HandleIdle(FrameTime frameTime, InputState inputState, float anchorOffset) {
        // TODO: should check if the user is suddenly falling due to a wall pushing them, a platform falling, or an elevator rising etc
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, inputState, anchorOffset);

        if (inputState.IsKeyNewlyPressed(Keys.Space)) {
            return this.GetJumpingState(horizontalVelocity, movementDirection);
        }

        if (!this.CheckIfStillGrounded(anchorOffset, out _)) {
            return this.GetFallingState(frameTime, horizontalVelocity, movementDirection);
        }

        return horizontalVelocity != 0f ? new ActorState(MovementKind.Moving, movementDirection, this.CurrentState.Position, new Vector2(horizontalVelocity, 0f)) : this.CurrentState;
    }

    /// <inheritdoc />
    protected override ActorState HandleJumping(FrameTime frameTime, InputState inputState, float anchorOffset) {
        var verticalVelocity = this.JumpVelocity;
        this._elapsedJumpSeconds += (float)frameTime.SecondsPassed;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, inputState, anchorOffset);
        this.MovementDirection = movementDirection;
        var movementKind = this.CurrentState.MovementKind;


        if (this.CheckIfHitCeiling(frameTime, verticalVelocity, anchorOffset)) {
            verticalVelocity = 0f;
            movementKind = MovementKind.Falling;
            this._elapsedJumpSeconds = 0f;
        }
        else if (!inputState.IsKeyHeld(Keys.Space) || this._elapsedJumpSeconds > this.JumpHoldTime) {
            movementKind = MovementKind.Falling;
            this._elapsedJumpSeconds = 0f;
        }

        return new ActorState(movementKind, movementDirection, this.Transform.Position, new Vector2(horizontalVelocity, verticalVelocity));
    }

    /// <inheritdoc />
    protected override ActorState HandleMoving(FrameTime frameTime, InputState inputState, float anchorOffset) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, inputState, anchorOffset);
        this.MovementDirection = movementDirection;
        var movementKind = this.CurrentState.MovementKind;

        // TODO: this could work for slopes if we use the raycast hit
        if (!this.CheckIfStillGrounded(anchorOffset, out _)) {
            return this.GetFallingState(frameTime, horizontalVelocity, movementDirection);
        }

        if (inputState.IsKeyNewlyPressed(Keys.Space)) {
            return this.GetJumpingState(horizontalVelocity, movementDirection);
        }

        if (horizontalVelocity == 0f) {
            return new ActorState(MovementKind.Idle, movementDirection, this.Transform.Position, Vector2.Zero);
        }

        return new ActorState(movementKind, movementDirection, this.Transform.Position, new Vector2(horizontalVelocity, 0f));
    }

    // TODO: need to make this generic for gamepads/keyboards/user settings
    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime, InputState inputState, float anchorOffset) {
        var horizontalAxis = 0f;
        if (inputState.IsKeyHeld(Keys.A)) {
            horizontalAxis = -1f;
        }
        else if (inputState.IsKeyHeld(Keys.D)) {
            horizontalAxis = 1f;
        }

        var previousVelocity = this.CurrentState.Velocity.X;
        var horizontalVelocity = previousVelocity;
        var movingDirection = this.CurrentState.FacingDirection;

        if (horizontalAxis > 0f) {
            var newVelocity = horizontalVelocity + this.GetHorizontalAcceleration() * (float)frameTime.SecondsPassed;
            horizontalVelocity = previousVelocity >= 0f ? Math.Max(newVelocity, this.MaximumHorizontalVelocity * 0.5f) : newVelocity;
            movingDirection = HorizontalDirection.Right;
        }
        else if (horizontalVelocity > 0f) {
            horizontalVelocity -= this.DecelerationRate * (float)frameTime.SecondsPassed;

            if (horizontalVelocity < this.MinimumVelocity) {
                horizontalVelocity = 0f;
            }
        }

        if (horizontalAxis < 0f) {
            var newVelocity = horizontalVelocity - this.GetHorizontalAcceleration() * (float)frameTime.SecondsPassed;
            horizontalVelocity = previousVelocity <= 0f ? Math.Min(newVelocity, -this.MaximumHorizontalVelocity * 0.5f) : newVelocity;
            movingDirection = HorizontalDirection.Left;
        }
        else if (horizontalVelocity < 0f) {
            horizontalVelocity += this.DecelerationRate * (float)frameTime.SecondsPassed;

            if (horizontalVelocity > -this.MinimumVelocity) {
                horizontalVelocity = 0f;
            }
        }

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