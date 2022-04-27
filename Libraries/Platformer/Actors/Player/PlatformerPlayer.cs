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
    private PlatformerCamera? _camera;
    private float _jumpHoldTime = 0.1f;
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;
    private QueueableSpriteAnimator? _spriteAnimator;

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

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.IdleAnimationReference.Initialize(this.Scene.Assets);
        this.MovingAnimationReference.Initialize(this.Scene.Assets);
        this.JumpingAnimationReference.Initialize(this.Scene.Assets);
        this.FallingAnimationReference.Initialize(this.Scene.Assets);

        this._camera = this.GetOrAddChild<PlatformerCamera>();
        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

        if (this.IdleAnimationReference.PackagedAsset is { } animation) {
            this._spriteAnimator.Play(animation, true);
        }

        this.CurrentState = new ActorState(StateType.Idle, this._spriteAnimator.RenderSettings.FlipHorizontal ? HorizontalDirection.Left : HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, 0f);
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this._input.Update(inputState);

        var previousState = this.CurrentState;
        this.CurrentState = this.GetNewActorState(frameTime);
        this.PreviousState = previousState;
        this.ResetFacingDirection();

        if (this.CurrentState.StateType != this.PreviousState.StateType) {
            this.ResetAnimation();
        }

        this._camera?.UpdateDesiredPosition(this.CurrentState, this.PreviousState, frameTime, this.IsOnPlatform);
    }

    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime) {
        var horizontalVelocity = this._input.HorizontalAxis * this.MaximumHorizontalVelocity;
        var requireAcceleration = this.ShouldRequireAcceleration();
        if (requireAcceleration) {
            horizontalVelocity = this._input.HorizontalAxis switch {
                < 0f when this.PreviousState.Velocity.X > 0f => Math.Max(this.PreviousState.Velocity.X - (float)frameTime.SecondsPassed * this.AccelerationWhenChangingDirection, -this.MaximumHorizontalVelocity),
                > 0f when this.PreviousState.Velocity.X < 0f => Math.Min(this.PreviousState.Velocity.X + (float)frameTime.SecondsPassed * this.AccelerationWhenChangingDirection, this.MaximumHorizontalVelocity),
                _ => horizontalVelocity
            };
        }

        var movingDirection = this._input.HorizontalAxis switch {
            > 0f => HorizontalDirection.Right,
            < 0f => HorizontalDirection.Left,
            _ => this.CurrentState.FacingDirection
        };

        if (horizontalVelocity != 0f) {
            if (this.CheckIfHitWall(frameTime, horizontalVelocity, true)) {
                horizontalVelocity = 0f;
            }

            this.CheckIfHitWall(frameTime, -horizontalVelocity, false);
        }
        else {
            this.CheckIfHitWall(frameTime, 0f, false);
        }

        if (horizontalVelocity == 0f && requireAcceleration) {
            horizontalVelocity = this.PreviousState.Velocity.X switch {
                > 0f => (float)Math.Max(0f, this.PreviousState.Velocity.X - frameTime.SecondsPassed * this.Deceleration),
                < 0f => (float)Math.Min(0f, this.PreviousState.Velocity.X + frameTime.SecondsPassed * this.Deceleration),
                _ => horizontalVelocity
            };
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

    private float GetSecondsInState(FrameTime frameTime, StateType newState) {
        var secondsPassed = (float)frameTime.SecondsPassed;
        if (this.CurrentState.StateType == newState) {
            secondsPassed += this.CurrentState.SecondsInState;
        }

        return secondsPassed;
    }

    private ActorState HandleFalling(FrameTime frameTime) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        StateType stateType;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);

        if (verticalVelocity < 0f && this.CheckIfHitGround(frameTime, verticalVelocity, out var groundEntity)) {
            if (groundEntity is IBouncePlatform { BounceVelocity: > 0f } bouncePlatform) {
                verticalVelocity = bouncePlatform.BounceVelocity;
                stateType = StateType.Jumping;

                if (this._input.JumpState is ButtonState.Held or ButtonState.Pressed) {
                    verticalVelocity = Math.Max(this.JumpVelocity * 0.5f + verticalVelocity, this.JumpVelocity);
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

    private ActorState HandleIdle(FrameTime frameTime) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        var verticalVelocity = 0f;
        StateType stateType;

        if (this._input.JumpState == ButtonState.Pressed) {
            this.UnsetPlatform();
            stateType = StateType.Jumping;
            verticalVelocity = this.JumpVelocity;
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

    private ActorState HandleJumping(FrameTime frameTime) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        StateType stateType;

        if (this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
            verticalVelocity = 0f;
            stateType = StateType.Falling;
        }
        else if (this._input.JumpState != ButtonState.Held || this.CurrentState.SecondsInState > this.JumpHoldTime) {
            stateType = StateType.Falling;
        }
        else {
            stateType = StateType.Jumping;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime, stateType));
    }

    private ActorState HandleMoving(FrameTime frameTime) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        var verticalVelocity = 0f;
        StateType stateType;

        if (this._input.JumpState == ButtonState.Pressed) {
            this.UnsetPlatform();
            stateType = StateType.Jumping;
            verticalVelocity = this.JumpVelocity;
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

    private void ResetAnimation() {
        if (this._spriteAnimator != null) {
            if (this.CurrentState.StateType != this.PreviousState.StateType) {
                var spriteAnimation = this.CurrentState.StateType switch {
                    StateType.Idle => this.IdleAnimationReference.PackagedAsset,
                    StateType.Moving => this.MovingAnimationReference.PackagedAsset,
                    StateType.Falling => this.FallingAnimationReference.PackagedAsset,
                    StateType.Jumping => this.JumpingAnimationReference.PackagedAsset,
                    _ => null
                };

                if (spriteAnimation != null) {
                    this._spriteAnimator.Play(spriteAnimation, true);
                }
            }
        }
    }

    private void ResetFacingDirection() {
        if (this.CurrentState.FacingDirection != this.PreviousState.FacingDirection && this._spriteAnimator != null) {
            this._spriteAnimator.RenderSettings.FlipHorizontal = this.CurrentState.FacingDirection == HorizontalDirection.Left;
        }
    }

    private bool ShouldRequireAcceleration() {
        return this.CurrentState.StateType is StateType.Falling or StateType.Moving;
    }
}