namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public class PlayerPlatformerActor : PlatformerActor {
    /// <inheritdoc />
    protected override ActorState HandleFalling(FrameTime frameTime, InputState inputState) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override ActorState HandleIdle(FrameTime frameTime, InputState inputState) {
        // TODO: should check if the user is suddenly falling due to a wall pushing them, a platform falling, or an elevator rising etc
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, inputState);
        this.MovementDirection = movementDirection;
        return horizontalVelocity != 0f ?
            new ActorState(MovementKind.Moving, this.CurrentState.Position, new Vector2(horizontalVelocity, 0f)) : 
            this.CurrentState;
    }

    /// <inheritdoc />
    protected override ActorState HandleJumping(FrameTime frameTime, InputState inputState) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, inputState);
        this.MovementDirection = movementDirection;

        if (!this.CheckIfStillGrounded(out _)) {
            var verticalVelocity = -this.PhysicsSystem.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            return new ActorState(MovementKind.Falling, this.Transform.Position, new Vector2(horizontalVelocity, verticalVelocity));
        }

        // TODO: longer jumps when held and maybe it doesn't make sense to check the jump button for any other reason in here
        return inputState.IsKeyNewlyReleased(Keys.Space) ? 
            new ActorState(MovementKind.Jumping, this.Transform.Position, new Vector2(horizontalVelocity, this.JumpVelocity)) : 
            this.CurrentState;
    }


    /// <inheritdoc />
    protected override ActorState HandleMoving(FrameTime frameTime, InputState inputState) {
        throw new NotImplementedException();
    }

    // TODO: need to make this generic for gamepads/keyboards/user settings
    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime, InputState inputState) {
        var horizontalAxis = 0f;
        if (inputState.IsKeyHeld(Keys.A)) {
            horizontalAxis = -1f;
        }
        else if (inputState.IsKeyHeld(Keys.D)) {
            horizontalAxis = 1f;
        }

        var previousVelocity = this.CurrentState.Velocity.X;
        var horizontalVelocity = previousVelocity;
        var movingDirection = HorizontalDirection.None;

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
            movingDirection = movingDirection == HorizontalDirection.None ? HorizontalDirection.Left : HorizontalDirection.None;
        }
        else if (horizontalVelocity < 0f) {
            horizontalVelocity += this.DecelerationRate * (float)frameTime.SecondsPassed;

            if (horizontalVelocity > -this.MinimumVelocity) {
                horizontalVelocity = 0f;
            }
        }

        if (horizontalVelocity != 0f) {
            if (this.CheckIfHitWall(frameTime, horizontalVelocity, true)) {
                horizontalVelocity = 0f;
            }

            this.CheckIfHitWall(frameTime, -horizontalVelocity, false);
        }
        else {
            this.CheckIfHitWall(frameTime, 0f, false);
        }

        return (horizontalVelocity, movingDirection);
    }
}