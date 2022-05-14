namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public class PlatformerPlayer : PlatformerActor {
    private PlatformerCamera? _camera;
    private float _jumpHoldTime = 0.1f;
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;

    /// <summary>
    /// Gets the falling animation reference.
    /// </summary>
    [DataMember(Order = 13, Name = "Falling Animation")]
    public SpriteAnimationReference FallingAnimationReference { get; } = new();

    /// <summary>
    /// Gets the idle animation reference.
    /// </summary>
    [DataMember(Order = 10, Name = "Idle Animation")]
    public SpriteAnimationReference IdleAnimationReference { get; } = new();

    /// <summary>
    /// Gets the jumping animation reference.
    /// </summary>
    [DataMember(Order = 12, Name = "Jumping Animation")]
    public SpriteAnimationReference JumpingAnimationReference { get; } = new();

    /// <summary>
    /// Gets the moving animation reference.
    /// </summary>
    [DataMember(Order = 11, Name = "Moving Animation")]
    public SpriteAnimationReference MovingAnimationReference { get; } = new();

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
    /// Gets or sets the initial velocity of a jump for this actor.
    /// </summary>
    [DataMember]
    [Category("Movement")]
    public float JumpVelocity {
        get => this._jumpVelocity;
        set => this.Set(ref this._jumpVelocity, value);
    }

    /// <summary>
    /// Gets or sets the maximum horizontal velocity allowed.
    /// </summary>
    [DataMember]
    [Category("Movement")]
    public float MaximumHorizontalVelocity {
        get => this._maximumHorizontalVelocity;
        set => this.Set(ref this._maximumHorizontalVelocity, value);
    }

    /// <summary>
    /// Gets the input manager.
    /// </summary>
    protected virtual InputManager Input { get; } = new();

    /// <summary>
    /// Gets the sprite animator.
    /// </summary>
    protected QueueableSpriteAnimator? SpriteAnimator { get; private set; }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.IdleAnimationReference.Initialize(this.Scene.Assets);
        this.MovingAnimationReference.Initialize(this.Scene.Assets);
        this.JumpingAnimationReference.Initialize(this.Scene.Assets);
        this.FallingAnimationReference.Initialize(this.Scene.Assets);

        this._camera = this.GetOrAddChild<PlatformerCamera>();
        this.SpriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this.SpriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

        if (this.IdleAnimationReference.PackagedAsset is { } animation) {
            this.SpriteAnimator.Play(animation, true);
        }

        this.CurrentState = new ActorState(StateType.Idle, this.SpriteAnimator.RenderSettings.FlipHorizontal ? HorizontalDirection.Left : HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, 0f);
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, Framework.InputState inputState) {
        this.Input.Update(inputState);

        var previousState = this.CurrentState;
        this.CurrentState = this.GetNewActorState(frameTime);
        this.PreviousState = previousState;
        this.ResetFacingDirection();

        if (this.ShouldResetAnimation()) {
            this.ResetAnimation();
        }

        this._camera?.UpdateDesiredPosition(this.CurrentState, this.PreviousState, frameTime, this.IsOnPlatform);
    }

    /// <summary>
    /// Gets the jump velocity;
    /// </summary>
    /// <returns>The jump velocity.</returns>
    protected virtual float GetJumpVelocity() {
        return this.JumpVelocity;
    }

    /// <summary>
    /// Gets the seconds in the state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="newState">The new state.</param>
    /// <returns>The seconds in the specified state.</returns>
    protected float GetSecondsInState(FrameTime frameTime, StateType newState) {
        var secondsPassed = (float)frameTime.SecondsPassed;
        if (this.CurrentState.StateType == newState) {
            secondsPassed += this.CurrentState.SecondsInState;
        }

        return secondsPassed;
    }

    /// <summary>
    /// Handles falling and determines the actor's state after the current frame.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <returns>The new actor state.</returns>
    protected virtual ActorState HandleFalling(FrameTime frameTime) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        StateType stateType;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);

        if (verticalVelocity < 0f && this.CheckIfHitGround(frameTime, verticalVelocity, out var groundEntity)) {
            if (groundEntity is IBouncePlatform { BounceVelocity: > 0f } bouncePlatform) {
                verticalVelocity = bouncePlatform.BounceVelocity;
                stateType = StateType.Jumping;

                if (this.Input.JumpState is InputState.Held or InputState.Pressed) {
                    var jumpVelocity = this.GetJumpVelocity();
                    verticalVelocity = Math.Max(jumpVelocity * 0.5f + verticalVelocity, jumpVelocity);
                }
            }
            else {
                stateType = horizontalVelocity != 0f ? StateType.Moving : StateType.Idle;
                verticalVelocity = 0f;
            }
        }
        else {
            if (verticalVelocity > 0f && this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
                verticalVelocity = 0f;
            }

            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            verticalVelocity = Math.Max(-this.PhysicsLoop.TerminalVelocity, verticalVelocity);
            stateType = StateType.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime, stateType));
    }

    /// <summary>
    /// Handles the idle stance and determines the actor's state after the current frame.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <returns>The new actor state.</returns>
    protected virtual ActorState HandleIdle(FrameTime frameTime) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        var verticalVelocity = 0f;
        StateType stateType;

        if (this.Input.JumpState == InputState.Pressed) {
            this.UnsetPlatform();
            stateType = StateType.Jumping;
            verticalVelocity = this.GetJumpVelocity();
        }
        else if (this.CheckIfStillGrounded()) {
            stateType = horizontalVelocity != 0f ? StateType.Moving : StateType.Idle;
        }
        else {
            stateType = StateType.Falling;
            verticalVelocity = this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime, stateType));
    }

    /// <summary>
    /// Handles jumping and determines the actor's state after the current frame.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <returns>The new actor state.</returns>
    protected virtual ActorState HandleJumping(FrameTime frameTime) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        StateType stateType;

        if (this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
            verticalVelocity = 0f;
            stateType = StateType.Falling;
        }
        else if (this.Input.JumpState != InputState.Held || this.CurrentState.SecondsInState > this.JumpHoldTime) {
            stateType = StateType.Falling;
        }
        else {
            stateType = StateType.Jumping;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime, stateType));
    }

    /// <summary>
    /// Handles moving and determines the actor's state after the current frame.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <returns>The new actor state.</returns>
    protected virtual ActorState HandleMoving(FrameTime frameTime) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        var verticalVelocity = 0f;
        StateType stateType;

        if (this.Input.JumpState == InputState.Pressed) {
            this.UnsetPlatform();
            stateType = StateType.Jumping;
            verticalVelocity = this.GetJumpVelocity();
        }
        else if (this.CheckIfStillGrounded()) {
            stateType = horizontalVelocity != 0f ? StateType.Moving : StateType.Idle;
        }
        else {
            verticalVelocity = this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            stateType = StateType.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime, stateType));
    }

    /// <summary>
    /// Resets the animation according to the current state.
    /// </summary>
    protected virtual void ResetAnimation() {
        var spriteAnimation = this.CurrentState.StateType switch {
            StateType.Idle => this.IdleAnimationReference.PackagedAsset,
            StateType.Moving => this.MovingAnimationReference.PackagedAsset,
            StateType.Falling => this.FallingAnimationReference.PackagedAsset,
            StateType.Jumping => this.JumpingAnimationReference.PackagedAsset,
            _ => null
        };

        if (spriteAnimation != null) {
            this.SpriteAnimator?.Play(spriteAnimation, true);
        }
    }

    /// <summary>
    /// Checks whether the animation should be reset.
    /// </summary>
    /// <returns>A value indicating whether or not the animation should be reset.</returns>
    protected virtual bool ShouldResetAnimation() {
        return this.CurrentState.StateType != this.PreviousState.StateType && this.SpriteAnimator != null;
    }

    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime) {
        var horizontalVelocity = this.Input.HorizontalAxis * this.MaximumHorizontalVelocity;
        var movingDirection = this.CurrentState.FacingDirection;
        
        if (horizontalVelocity < 0f) {
            horizontalVelocity = Math.Min(horizontalVelocity, this.CurrentState.Velocity.X);
            movingDirection = HorizontalDirection.Left;
        }
        else if (horizontalVelocity > 0f) {
            horizontalVelocity = Math.Max(horizontalVelocity, this.CurrentState.Velocity.X);
            movingDirection = HorizontalDirection.Right;
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

    private ActorState GetNewActorState(FrameTime frameTime) {
        if (this.Size.X > 0f && this.Size.Y > 0f) {
            return this.CurrentState.StateType switch {
                StateType.Idle => this.HandleIdle(frameTime),
                StateType.Moving => this.HandleMoving(frameTime),
                StateType.Jumping => this.HandleJumping(frameTime),
                StateType.Falling => this.HandleFalling(frameTime),
                _ => this.CurrentState
            };
        }

        return this.CurrentState;
    }

    private void ResetFacingDirection() {
        if (this.CurrentState.FacingDirection != this.PreviousState.FacingDirection && this.SpriteAnimator != null) {
            this.SpriteAnimator.RenderSettings.FlipHorizontal = this.CurrentState.FacingDirection == HorizontalDirection.Left;
        }
    }
}