namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Describes a player's kind of movement.
/// </summary>
public enum PlayerMovement : byte {
    Idle,
    Moving,
    Jumping,
    Falling
}

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public sealed class PlatformerPlayer : PlatformerActor {
    private readonly InputManager _input = new();
    private PlatformerCamera? _camera;
    private PlayerMovement _currentPlayerMovement = PlayerMovement.Idle;
    private float _elapsedRunSeconds;
    private float _jumpHoldTime = 0.1f;
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;
    private float _originalMaximumHorizontalVelocity;
    private PlayerMovement _previousPlayerMovement = PlayerMovement.Idle;
    private QueueableSpriteAnimator? _spriteAnimator;
    private float _timeUntilRun = 1f;

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

    private PlayerMovement CurrentPlayerMovement {
        get => this._currentPlayerMovement;
        set {
            this._previousPlayerMovement = this._currentPlayerMovement;
            this._currentPlayerMovement = value;

            if (this._currentPlayerMovement != this._previousPlayerMovement) {
                this.ResetAnimation();
            }
        }
    }

    private float ElapsedRunSeconds {
        get => this._elapsedRunSeconds;
        set => this._elapsedRunSeconds = Math.Clamp(value, 0f, this.TimeUntilRun);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._originalMaximumHorizontalVelocity = this.MaximumHorizontalVelocity;

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

        this.CurrentState = new ActorState(this._spriteAnimator.RenderSettings.FlipHorizontal ? HorizontalDirection.Left : HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, 0f);
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this._input.Update(inputState);
        var isMovingFast = this.GetIsMovingFast();
        this.MaximumHorizontalVelocity = isMovingFast ? this.RunVelocity : this._originalMaximumHorizontalVelocity;

        var previousState = this.CurrentState;
        this.CurrentState = this.GetNewActorState(frameTime);
        this.PreviousState = previousState;
        this.ResetFacingDirection();
        this._camera?.UpdateDesiredPosition(this.CurrentState, this.PreviousState, frameTime, this.IsOnPlatform);
    }

    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime) {
        var horizontalVelocity = this._input.HorizontalAxis * this.MaximumHorizontalVelocity;

        if (this.ElapsedRunSeconds > 0f) {
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

    private bool GetIsMovingFast() {
        return this.ElapsedRunSeconds >= this.TimeUntilRun;
    }

    private ActorState GetNewActorState(FrameTime frameTime) {
        if (this.Size.X > 0f && this.Size.Y > 0f) {
            this.HandleBounce(frameTime);

            return this.CurrentPlayerMovement switch {
                PlayerMovement.Idle => this.HandleIdle(frameTime),
                PlayerMovement.Moving => this.HandleMoving(frameTime),
                PlayerMovement.Jumping => this.HandleJumping(frameTime),
                PlayerMovement.Falling => this.HandleFalling(frameTime),
                _ => this.CurrentState
            };
        }

        return this.CurrentState;
    }

    private float GetSecondsInState(FrameTime frameTime) {
        var secondsPassed = (float)frameTime.SecondsPassed;
        if (this.CurrentPlayerMovement == this._previousPlayerMovement) {
            secondsPassed += this.CurrentState.SecondsInState;
        }

        return secondsPassed;
    }

    private void HandleBounce(FrameTime frameTime) {
        var velocity = this.BounceVelocity;
        if (velocity != Vector2.Zero) {
            this.ApplyVelocity(frameTime, this.BounceVelocity);
            if (velocity.X == 0f) {
                velocity = new Vector2(this.CurrentState.Velocity.X, velocity.Y);
                this.UnsetPlatform();
                this.CurrentPlayerMovement = velocity.Y > 0f ? PlayerMovement.Jumping : PlayerMovement.Falling;
            }
            else if (velocity.Y == 0f) {
                velocity = new Vector2(velocity.X, this.CurrentState.Velocity.Y);
            }

            this.PreviousState = this.CurrentState;
            this.CurrentState = new ActorState(this.CurrentState.FacingDirection, this.Transform.Position, velocity, (float)frameTime.SecondsPassed);
        }

        this.BounceVelocity = Vector2.Zero;
    }

    private ActorState HandleFalling(FrameTime frameTime) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);

        if (verticalVelocity < 0f && this.CheckIfHitGround(frameTime, verticalVelocity)) {
            this.CurrentPlayerMovement = horizontalVelocity != 0f ? PlayerMovement.Moving : PlayerMovement.Idle;
            verticalVelocity = 0f;
        }
        else {
            if (verticalVelocity > 0f && this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
                verticalVelocity = 0f;
            }

            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            verticalVelocity = Math.Max(-this.PhysicsLoop.TerminalVelocity, verticalVelocity);
            this.CurrentPlayerMovement = PlayerMovement.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleIdle(FrameTime frameTime) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        var verticalVelocity = 0f;

        if (this._input.JumpState == ButtonState.Pressed) {
            this.UnsetPlatform();
            this.CurrentPlayerMovement = PlayerMovement.Jumping;
            verticalVelocity = this.JumpVelocity;
        }
        else if (this.CheckIfStillGrounded()) {
            this.CurrentPlayerMovement = horizontalVelocity != 0f ? PlayerMovement.Moving : PlayerMovement.Idle;
        }
        else {
            this.CurrentPlayerMovement = PlayerMovement.Falling;
            verticalVelocity = this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleJumping(FrameTime frameTime) {
        var verticalVelocity = this.JumpVelocity;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);

        if (this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
            verticalVelocity = 0f;
            this.CurrentPlayerMovement = PlayerMovement.Falling;
        }
        else if (this._input.JumpState != ButtonState.Held || this.CurrentState.SecondsInState > this.JumpHoldTime) {
            this.CurrentPlayerMovement = PlayerMovement.Falling;
        }
        else {
            this.CurrentPlayerMovement = PlayerMovement.Jumping;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleMoving(FrameTime frameTime) {
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime);
        var verticalVelocity = 0f;

        if (this._input.JumpState == ButtonState.Pressed) {
            this.UnsetPlatform();
            this.CurrentPlayerMovement = PlayerMovement.Jumping;
            verticalVelocity = this.JumpVelocity;
        }
        else if (this.CheckIfStillGrounded()) {
            this.CurrentPlayerMovement = horizontalVelocity != 0f ? PlayerMovement.Moving : PlayerMovement.Idle;
        }
        else {
            verticalVelocity = this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            this.CurrentPlayerMovement = PlayerMovement.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }


    private void ResetAnimation() {
        if (this._spriteAnimator != null) {
            if (this.CurrentPlayerMovement != this._previousPlayerMovement) {
                var spriteAnimation = this.CurrentPlayerMovement switch {
                    PlayerMovement.Idle => this.IdleAnimationReference.PackagedAsset,
                    PlayerMovement.Moving => this.MovingAnimationReference.PackagedAsset,
                    PlayerMovement.Falling => this.FallingAnimationReference.PackagedAsset,
                    PlayerMovement.Jumping => this.JumpingAnimationReference.PackagedAsset,
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
}