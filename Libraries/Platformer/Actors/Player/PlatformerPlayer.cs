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
    private bool _isJumping;
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

        this.CurrentState = new ActorState(StateType.Grounded, this.SpriteAnimator.RenderSettings.FlipHorizontal ? HorizontalDirection.Left : HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, 0f);
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, Framework.InputState inputState) {
        this.Input.Update(inputState);

        var previousState = this.CurrentState;
        this.CurrentState = this.GetNewActorState(frameTime);
        this.PreviousState = previousState;
        this.ResetFacingDirection();
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
    protected virtual ActorState HandleAerial(FrameTime frameTime) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        StateType stateType;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);

        if (verticalVelocity < 0f && this.CheckIfHitGround(frameTime, verticalVelocity, out var groundEntity)) {
            if (groundEntity is IBouncePlatform { BounceVelocity: > 0f } bouncePlatform) {
                this._isJumping = true;
                verticalVelocity = bouncePlatform.BounceVelocity;
                stateType = StateType.Aerial;

                if (this.Input.JumpState is InputState.Held or InputState.Pressed) {
                    var jumpVelocity = this.GetJumpVelocity();
                    verticalVelocity = Math.Max(jumpVelocity * 0.5f + verticalVelocity, jumpVelocity);
                }
            }
            else {
                stateType = StateType.Grounded;
                this.PlayGroundedAnimation(horizontalVelocity);
                verticalVelocity = 0f;
            }
        }
        else {
            if (verticalVelocity > 0f && this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
                verticalVelocity = 0f;
                this._isJumping = false;
            }
            else if (this._isJumping && this.Input.JumpState == InputState.Held) {
                if (this.CurrentState.SecondsInState > this.JumpHoldTime) {
                    this._isJumping = false;
                }
            }
            else {
                verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
                verticalVelocity = Math.Max(-this.PhysicsLoop.TerminalVelocity, verticalVelocity);
            }


            stateType = StateType.Aerial;
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
    protected virtual ActorState HandleGrounded(FrameTime frameTime) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        var verticalVelocity = 0f;
        StateType stateType;

        if (this.Input.JumpState == InputState.Pressed) {
            verticalVelocity = this.Jump(out stateType);
        }
        else if (this.CheckIfStillGrounded()) {
            stateType = StateType.Grounded;
            this.PlayGroundedAnimation(horizontalVelocity);
        }
        else {
            verticalVelocity = this.Fall(frameTime, out stateType);
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime, stateType));
    }

    /// <summary>
    /// Gets a value indicating whether or not the grounded state has changed from an aerial state or if the horizontal velocity has changed between zero/non-zero.
    /// </summary>
    /// <param name="newHorizontalVelocity">The new horizontal velocity.</param>
    /// <returns>A value indicating whether or not the grounded state has changed.</returns>
    protected bool HasGroundedStateChanged(float newHorizontalVelocity) {
        return this.CurrentState.StateType == StateType.Aerial ||
               (newHorizontalVelocity == 0f && this.CurrentState.Velocity.X != 0f) ||
               (newHorizontalVelocity != 0f && this.CurrentState.Velocity.X == 0f);
    }

    /// <summary>
    /// Completely resets the current animation.
    /// </summary>
    protected virtual void ResetAnimation() {
        if (this.CurrentState.StateType == StateType.Aerial) {
            this.PlayFallAnimation();
        }
        else if (this.CurrentState.Velocity.X == 0f) {
            this.PlayIdleAnimation();
        }
        else {
            this.PlayMovingAnimation();
        }
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
            if (this.CheckIfHitWall(frameTime, horizontalVelocity, true, out _)) {
                horizontalVelocity = 0f;
            }
        }
        else {
            this.CheckIfHitWall(frameTime, 0f, false, out _);
        }

        return (horizontalVelocity, movingDirection);
    }

    private float Fall(FrameTime frameTime, out StateType stateType) {
        stateType = StateType.Aerial;
        this.UnsetPlatform();
        this.PlayFallAnimation();
        return this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
    }

    private ActorState GetNewActorState(FrameTime frameTime) {
        if (this.Size.X > 0f && this.Size.Y > 0f) {
            return this.CurrentState.StateType switch {
                StateType.Grounded => this.HandleGrounded(frameTime),
                StateType.Aerial => this.HandleAerial(frameTime),
                _ => this.CurrentState
            };
        }

        return this.CurrentState;
    }

    private float Jump(out StateType stateType) {
        this._isJumping = true;
        stateType = StateType.Aerial;
        this.UnsetPlatform();
        this.PlayJumpAnimation();
        return this.GetJumpVelocity();
    }

    private void PlayFallAnimation() {
        if (this.SpriteAnimator != null && this.FallingAnimationReference.PackagedAsset != null) {
            this.SpriteAnimator.Play(this.FallingAnimationReference.PackagedAsset, true);
        }
    }

    private void PlayGroundedAnimation(float horizontalVelocity) {
        if (this.HasGroundedStateChanged(horizontalVelocity)) {
            if (horizontalVelocity == 0f) {
                this.PlayIdleAnimation();
            }
            else {
                this.PlayMovingAnimation();
            }
        }
    }

    private void PlayIdleAnimation() {
        if (this.SpriteAnimator != null && this.IdleAnimationReference.PackagedAsset != null) {
            this.SpriteAnimator.Play(this.IdleAnimationReference.PackagedAsset, true);
        }
    }

    private void PlayJumpAnimation() {
        if (this.SpriteAnimator != null && this.JumpingAnimationReference.PackagedAsset != null && this.FallingAnimationReference.PackagedAsset != null) {
            this.SpriteAnimator.Play(this.JumpingAnimationReference.PackagedAsset, false);
            this.SpriteAnimator.Enqueue(this.FallingAnimationReference.PackagedAsset, true);
        }
    }

    private void PlayMovingAnimation() {
        if (this.SpriteAnimator != null && this.MovingAnimationReference.PackagedAsset != null) {
            this.SpriteAnimator.Play(this.MovingAnimationReference.PackagedAsset, true);
        }
    }

    private void ResetFacingDirection() {
        if (this.CurrentState.FacingDirection != this.PreviousState.FacingDirection && this.SpriteAnimator != null) {
            this.SpriteAnimator.RenderSettings.FlipHorizontal = this.CurrentState.FacingDirection == HorizontalDirection.Left;
        }
    }
}