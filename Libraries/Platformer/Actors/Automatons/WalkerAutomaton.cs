namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A simple automaton that moves forward until it hits a wall, then it moves the opposite direction.
/// </summary>
public class WalkerAutomaton : PlatformerActor {
    private bool _avoidLedges = true;
    private float _maxHorizontalVelocity = 1f;
    private float _pauseTime = 1f;
    private QueueableSpriteAnimator? _spriteAnimator;

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

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.IdleAnimationReference.Initialize(this.Scene.Assets);
        this.FallingAnimationReference.Initialize(this.Scene.Assets);
        this.WalkingAnimationReference.Initialize(this.Scene.Assets);

        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;
        this.CurrentState = new ActorState(StateType.Moving, HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, 0f);

        if (this.WalkingAnimationReference.PackagedAsset is { } animation) {
            this._spriteAnimator.Play(animation, true);
        }
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, Framework.InputState inputState) {
        var previousState = this.CurrentState;
        this.CurrentState = this.CurrentState.StateType switch {
            StateType.Idle => this.HandleIdle(frameTime),
            StateType.Falling => this.HandleFalling(frameTime),
            StateType.Moving => this.HandleWalking(frameTime),
            _ => this.CurrentState
        };
        
        this.PreviousState = previousState;
        if (this.CurrentState.StateType != this.PreviousState.StateType) {
            this.ResetAnimation();
        }
        this.ResetFacingDirection();
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

    private ActorState HandleFalling(FrameTime frameTime) {
        var horizontalVelocity = this.GetHorizontalVelocity(this.CurrentState.FacingDirection);
        var verticalVelocity = this.CurrentState.Velocity.Y;
        StateType stateType;

        if (this.CheckIfHitWall(frameTime, horizontalVelocity, true)) {
            horizontalVelocity = 0f;
        }

        if (this.CheckIfHitGround(frameTime, verticalVelocity, out var groundEntity)) {
            if (groundEntity is IBouncePlatform { BounceVelocity: > 0f } bouncePlatform) {
                verticalVelocity = bouncePlatform.BounceVelocity;
                stateType = StateType.Falling;
            }
            else {
                verticalVelocity = 0f;
                stateType = horizontalVelocity != 0f ? StateType.Moving : StateType.Idle;
            }
        }
        else {
            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            verticalVelocity = Math.Max(-this.PhysicsLoop.TerminalVelocity, verticalVelocity);
            stateType = StateType.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, this.CurrentState.FacingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleIdle(FrameTime frameTime) {
        var horizontalVelocity = 0f;
        var verticalVelocity = 0f;
        var facingDirection = this.CurrentState.FacingDirection;
        StateType stateType;

        if (this.CurrentState.SecondsInState > this.PauseTime) {
            stateType = StateType.Moving;
            if (facingDirection == HorizontalDirection.Left) {
                facingDirection = HorizontalDirection.Right;
                horizontalVelocity = this.MaxHorizontalVelocity;
            }
            else {
                facingDirection = HorizontalDirection.Left;
                horizontalVelocity = -this.MaxHorizontalVelocity;
            }
        }
        else if (!this.CheckIfStillGrounded()) {
            stateType = StateType.Falling;
            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }
        else {
            stateType = StateType.Idle;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, facingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleWalking(FrameTime frameTime) {
        var horizontalVelocity = this.GetHorizontalVelocity(this.CurrentState.FacingDirection);
        var verticalVelocity = 0f;
        var facingDirection = this.CurrentState.FacingDirection;
        StateType stateType;

        if (this.CheckIfHitWall(frameTime, horizontalVelocity, true) || this.AvoidsLedges && this.CheckIfLedgeAhead(facingDirection)) {
            stateType = StateType.Idle;
            horizontalVelocity = 0f;
        }
        else if (!this.CheckIfStillGrounded()) {
            stateType = StateType.Falling;
            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }
        else {
            stateType = StateType.Moving;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(stateType, facingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private void ResetAnimation() {
        if (this._spriteAnimator != null) {
            if (this.CurrentState.StateType != this.PreviousState.StateType) {
                var spriteAnimation = this.CurrentState.StateType switch {
                    StateType.Idle => this.IdleAnimationReference.PackagedAsset,
                    StateType.Moving => this.WalkingAnimationReference.PackagedAsset,
                    StateType.Falling => this.FallingAnimationReference.PackagedAsset,
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