namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A simple automaton that moves forward until it hits a wall, then it moves the opposite direction.
/// </summary>
public class WalkerAutomaton : PlatformerActor {
    private bool _avoidLedges = true;
    private float _maxHorizontalVelocity = 1f;
    private float _pauseTime = 1f;
    private float _secondsSpentPaused;
    private QueueableSpriteAnimator? _spriteAnimator;

    /// <inheritdoc />
    public override bool CanAttachToWalls => false;

    /// <summary>
    /// Gets the falling animation reference.
    /// </summary>
    [DataMember(Order = 11, Name = "Falling Animation")]
    public SpriteAnimationReference FallingAnimationReference { get; } = new();

    /// <summary>
    /// Gets the idle animation reference.
    /// </summary>
    [DataMember(Order = 10, Name = "Idle Animation")]
    public SpriteAnimationReference IdleAnimationReference { get; } = new();

    /// <summary>
    /// Gets the walking animation reference.
    /// </summary>
    [DataMember(Order = 12, Name = "Walking Animation")]
    public SpriteAnimationReference WalkingAnimationReference { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether or not this automaton should avoid ledges or if it should walk off of them.
    /// </summary>
    [DataMember]
    public bool AvoidsLedges {
        get => this._avoidLedges;
        set => this.Set(ref this._avoidLedges, value);
    }

    /// <summary>
    /// Gets or sets the velocity while walking.
    /// </summary>
    [DataMember]
    public float MaxHorizontalVelocity {
        get => this._maxHorizontalVelocity;
        set => this.Set(ref this._maxHorizontalVelocity, value);
    }

    /// <summary>
    /// Gets or sets the time to pause when the walker must change directions.
    /// </summary>
    [DataMember]
    public float PauseTime {
        get => this._pauseTime;
        set => this.Set(ref this._pauseTime, value);
    }

    private float SecondsSpentPaused {
        get => this._secondsSpentPaused;
        set => this._secondsSpentPaused = Math.Clamp(value, 0f, this.PauseTime);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.IdleAnimationReference.Initialize(this.Scene.Assets);
        this.FallingAnimationReference.Initialize(this.Scene.Assets);
        this.WalkingAnimationReference.Initialize(this.Scene.Assets);

        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;
        this.CurrentState = new ActorState(StateType.Grounded, HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, 0f);

        this.Walk(false, out _, out _, out _);
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        var previousState = this.CurrentState;
        this.CurrentState = this.CurrentState.StateType switch {
            StateType.Aerial => this.HandleAerial(frameTime),
            StateType.Grounded => this.HandleGrounded(frameTime),
            _ => this.CurrentState
        };

        this.PreviousState = previousState;
        this.ResetFacingDirection();
    }

    private void Fall(FrameTime frameTime, out StateType stateType, out float horizontalVelocity, out float verticalVelocity) {
        stateType = StateType.Aerial;
        verticalVelocity = this.GetFrameGravity(frameTime);
        horizontalVelocity = 0f;
        this.SecondsSpentPaused = this.PauseTime;
        this.PlayIdleAnimation();
    }

    private float GetHorizontalVelocity(HorizontalDirection facingDirection) {
        return facingDirection == HorizontalDirection.Left ? -this.MaxHorizontalVelocity : this.MaxHorizontalVelocity;
    }

    private float GetSecondsInState(FrameTime frameTime) {
        var secondsPassed = (float)frameTime.SecondsPassed;
        if (this.CurrentState.StateType == this.PreviousState.StateType) {
            secondsPassed += this.CurrentState.SecondsInState;
        }

        return secondsPassed;
    }

    private ActorState HandleAerial(FrameTime frameTime) {
        var horizontalVelocity = this.GetHorizontalVelocity(this.CurrentState.FacingDirection);
        var verticalVelocity = this.CurrentState.Velocity.Y;
        StateType stateType;

        this.CheckIfHitWall(frameTime, horizontalVelocity, out horizontalVelocity, out _);

        if (this.CheckIfHitGround(frameTime, verticalVelocity, out var groundEntity)) {
            if (groundEntity is IBouncePlatform { BounceVelocity: > 0f } bouncePlatform) {
                verticalVelocity = bouncePlatform.BounceVelocity;
                stateType = StateType.Aerial;
            }
            else {
                verticalVelocity = 0f;
                this.Walk(false, out stateType, out horizontalVelocity, out _);
            }
        }
        else {
            verticalVelocity += this.GetFrameGravity(frameTime);
            verticalVelocity = Math.Max(-this.PhysicsLoop.TerminalVelocity, verticalVelocity);
            stateType = StateType.Aerial;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, this.CurrentState.FacingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleGrounded(FrameTime frameTime) {
        var isIdling = this.SecondsSpentPaused < this.PauseTime;
        var horizontalVelocity = isIdling ? 0f : this.GetHorizontalVelocity(this.CurrentState.FacingDirection);
        var verticalVelocity = 0f;
        var facingDirection = this.CurrentState.FacingDirection;
        StateType stateType;

        if (isIdling) {
            this.SecondsSpentPaused += (float)frameTime.SecondsPassed;
            stateType = StateType.Grounded;

            if (this.SecondsSpentPaused >= this.PauseTime) {
                this.Walk(true, out stateType, out horizontalVelocity, out facingDirection);
            }
        }
        else if (this.CheckIfHitWall(frameTime, horizontalVelocity, out horizontalVelocity, out _) || (this.AvoidsLedges && this.CheckIfLedgeAhead(facingDirection))) {
            this.Idle(out stateType, out horizontalVelocity);
        }
        else if (!this.CheckIfStillGrounded(frameTime, out _)) {
            this.Fall(frameTime, out stateType, out horizontalVelocity, out verticalVelocity);
        }
        else {
            stateType = StateType.Grounded;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, facingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private void Idle(out StateType stateType, out float horizontalVelocity) {
        this.SecondsSpentPaused = 0f;
        stateType = StateType.Grounded;
        horizontalVelocity = 0f;
        this.PlayIdleAnimation();
    }

    private void PlayIdleAnimation() {
        if (this._spriteAnimator != null && this.IdleAnimationReference.PackagedAsset != null) {
            this._spriteAnimator.Play(this.IdleAnimationReference.PackagedAsset, true);
        }
    }

    private void PlayWalkingAnimation() {
        if (this._spriteAnimator != null && this.WalkingAnimationReference.PackagedAsset != null) {
            this._spriteAnimator.Play(this.WalkingAnimationReference.PackagedAsset, true);
        }
    }

    private void ResetFacingDirection() {
        if (this.CurrentState.FacingDirection != this.PreviousState.FacingDirection && this._spriteAnimator != null) {
            this._spriteAnimator.RenderSettings.FlipHorizontal = this.CurrentState.FacingDirection == HorizontalDirection.Left;
        }
    }

    private void Walk(bool swapDirection, out StateType stateType, out float horizontalVelocity, out HorizontalDirection facingDirection) {
        stateType = StateType.Grounded;
        if (swapDirection) {
            facingDirection = this.CurrentState.FacingDirection == HorizontalDirection.Left ? HorizontalDirection.Right : HorizontalDirection.Left;
        }
        else {
            facingDirection = this.CurrentState.FacingDirection;
        }

        horizontalVelocity = facingDirection == HorizontalDirection.Left ? -this.MaxHorizontalVelocity : this.MaxHorizontalVelocity;
        this.SecondsSpentPaused = this.PauseTime;
        this.PlayWalkingAnimation();
    }
}