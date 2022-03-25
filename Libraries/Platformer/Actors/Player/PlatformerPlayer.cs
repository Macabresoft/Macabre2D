namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An implementation of <see cref="IPlatformerActor" /> for the player.
/// </summary>
public sealed class PlatformerPlayer : PlatformerActor {
    private readonly InputManager _input = new();
    private PlatformerCamera? _camera;
    private MovementKind _currentMovementKind = MovementKind.Idle;
    private float _elapsedRunSeconds;
    private float _jumpHoldTime = 0.1f;
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;
    private float _originalMaximumHorizontalVelocity;
    private MovementKind _previousMovementKind = MovementKind.Idle;
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

    private MovementKind CurrentMovementKind {
        get => this._currentMovementKind;
        set {
            this._previousMovementKind = this._currentMovementKind;
            this._currentMovementKind = value;

            if (this._currentMovementKind != this._previousMovementKind) {
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

        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.IdleAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.MovingAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.JumpingAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.FallingAnimationReference);

        this._camera = this.GetOrAddChild<PlatformerCamera>();
        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

        if (this.IdleAnimationReference.PackagedAsset is { } animation) {
            this._spriteAnimator.Play(animation, true);
        }
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this._input.Update(inputState);
        var isMovingFast = this.GetIsMovingFast();
        this.MaximumHorizontalVelocity = isMovingFast ? this.RunVelocity : this._originalMaximumHorizontalVelocity;

        var anchorOffset = this.Size.Y * this.Scene.Game.Project.Settings.InversePixelsPerUnit;
        this.PreviousState = this.CurrentState;
        this.CurrentState = this.GetNewActorState(frameTime, anchorOffset);
        this.ResetFacingDirection();
        this._camera?.UpdateDesiredPosition(this.CurrentState, this.PreviousState, frameTime);
    }

    private (float HorizontalVelocity, HorizontalDirection MovementDirection) CalculateHorizontalVelocity(FrameTime frameTime, float anchorOffset) {
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

    private bool GetIsMovingFast() {
        return this.ElapsedRunSeconds >= this.TimeUntilRun;
    }

    private ActorState GetNewActorState(FrameTime frameTime, float anchorOffset) {
        if (this.Size.X > 0f && this.Size.Y > 0f) {
            return this.CurrentMovementKind switch {
                MovementKind.Idle => this.HandleIdle(frameTime, anchorOffset),
                MovementKind.Moving => this.HandleMoving(frameTime, anchorOffset),
                MovementKind.Jumping => this.HandleJumping(frameTime, anchorOffset),
                MovementKind.Falling => this.HandleFalling(frameTime, anchorOffset),
                _ => this.CurrentState
            };
        }

        return this.CurrentState;
    }

    private float GetSecondsInState(FrameTime frameTime) {
        var secondsPassed = (float)frameTime.SecondsPassed;
        if (this.CurrentMovementKind == this._previousMovementKind) {
            secondsPassed += this.CurrentState.SecondsInState;
        }

        return secondsPassed;
    }

    private ActorState HandleFalling(FrameTime frameTime, float anchorOffset) {
        var verticalVelocity = this.CurrentState.Velocity.Y;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);

        if (verticalVelocity < 0f && this.CheckIfHitGround(frameTime, verticalVelocity, anchorOffset, out var hit)) {
            this.SetWorldPosition(new Vector2(this.Transform.Position.X, hit.ContactPoint.Y + this.HalfSize.Y));
            this.CurrentMovementKind = horizontalVelocity != 0f ? MovementKind.Moving : MovementKind.Idle;
            verticalVelocity = 0f;
        }
        else {
            if (verticalVelocity > 0f && this.CheckIfHitCeiling(frameTime, verticalVelocity, anchorOffset)) {
                verticalVelocity = 0f;
            }

            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            verticalVelocity = Math.Max(-this.PhysicsLoop.TerminalVelocity, verticalVelocity);
            this.CurrentMovementKind = MovementKind.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleIdle(FrameTime frameTime, float anchorOffset) {
        this.MoveWithPlatform();
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);
        var verticalVelocity = 0f;

        if (this._input.JumpState == ButtonState.Pressed) {
            this.CurrentMovementKind = MovementKind.Jumping;
            verticalVelocity = this.JumpVelocity;
        }
        else if (this.CheckIfStillGrounded(anchorOffset, out _)) {
            this.CurrentMovementKind = horizontalVelocity != 0f ? MovementKind.Moving : MovementKind.Idle;
        }
        else {
            this.CurrentMovementKind = MovementKind.Falling;
            verticalVelocity = -this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleJumping(FrameTime frameTime, float anchorOffset) {
        var verticalVelocity = this.JumpVelocity;
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);

        if (this.CheckIfHitCeiling(frameTime, verticalVelocity, anchorOffset)) {
            verticalVelocity = 0f;
            this.CurrentMovementKind = MovementKind.Falling;
        }
        else if (this._input.JumpState != ButtonState.Held || this.CurrentState.SecondsInState > this.JumpHoldTime) {
            this.CurrentMovementKind = MovementKind.Falling;
        }
        else {
            this.CurrentMovementKind = MovementKind.Jumping;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleMoving(FrameTime frameTime, float anchorOffset) {
        this.MoveWithPlatform();
        var (horizontalVelocity, movementDirection) = this.CalculateHorizontalVelocity(frameTime, anchorOffset);
        var verticalVelocity = 0f;

        if (this._input.JumpState == ButtonState.Pressed) {
            this.CurrentMovementKind = MovementKind.Jumping;
            verticalVelocity = this.JumpVelocity;
        }
        else if (this.CheckIfStillGrounded(anchorOffset, out _)) {
            this.CurrentMovementKind = horizontalVelocity != 0f ? MovementKind.Moving : MovementKind.Idle;
        }
        else {
            verticalVelocity = -this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            this.CurrentMovementKind = MovementKind.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(movementDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }


    private void ResetAnimation() {
        if (this._spriteAnimator != null) {
            if (this.CurrentMovementKind != this._previousMovementKind) {
                var spriteAnimation = this.CurrentMovementKind switch {
                    MovementKind.Idle => this.IdleAnimationReference.PackagedAsset,
                    MovementKind.Moving => this.MovingAnimationReference.PackagedAsset,
                    MovementKind.Falling => this.FallingAnimationReference.PackagedAsset,
                    MovementKind.Jumping => this.JumpingAnimationReference.PackagedAsset,
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