namespace Macabresoft.Macabre2D.Libraries.Platformer.Automatons;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Defines ways walkers can move.
/// </summary>
public enum WalkerMovement {
    Idle,
    Falling,
    Walking
}

/// <summary>
/// A simple automaton that moves forward until it hits a wall, then it moves the opposite direction.
/// </summary>
public class WalkerAutomaton : PlatformerActor {
    private bool _avoidLedges = true;
    private WalkerMovement _currentMovement = WalkerMovement.Walking;
    private float _maxHorizontalVelocity = 1f;
    private float _pauseTime = 1f;
    private WalkerMovement _previousMovement = WalkerMovement.Idle;
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

    private WalkerMovement CurrentMovement {
        get => this._currentMovement;
        set {
            this._previousMovement = this._currentMovement;
            this._currentMovement = value;

            if (this._currentMovement != this._previousMovement) {
                this.ResetAnimation();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.IdleAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.FallingAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.WalkingAnimationReference);

        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;
        this.CurrentState = new ActorState(HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, 0f);

        if (this.WalkingAnimationReference.PackagedAsset is { } animation) {
            this._spriteAnimator.Play(animation, true);
        }
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this.PreviousState = this.CurrentState;
        var anchorOffset = this.Size.Y * this.Settings.InversePixelsPerUnit;
        this.CurrentState = this.CurrentMovement switch {
            WalkerMovement.Idle => this.HandleIdle(frameTime, anchorOffset),
            WalkerMovement.Falling => this.HandleFalling(frameTime, anchorOffset),
            WalkerMovement.Walking => this.HandleWalking(frameTime, anchorOffset),
            _ => this.CurrentState
        };

        this.ResetFacingDirection();
    }

    private float GetHorizontalVelocity(HorizontalDirection facingDirection) {
        return facingDirection == HorizontalDirection.Left ? -this.MaxHorizontalVelocity : this.MaxHorizontalVelocity;
    }

    private float GetSecondsInState(FrameTime frameTime) {
        var secondsPassed = (float)frameTime.SecondsPassed;
        if (this.CurrentMovement == this._previousMovement) {
            secondsPassed += this.CurrentState.SecondsInState;
        }

        return secondsPassed;
    }

    private ActorState HandleFalling(FrameTime frameTime, float anchorOffset) {
        var horizontalVelocity = this.GetHorizontalVelocity(this.CurrentState.FacingDirection);
        var verticalVelocity = this.CurrentState.Velocity.Y;

        if (this.CheckIfHitWall(frameTime, horizontalVelocity, true, anchorOffset)) {
            horizontalVelocity = 0f;
        }

        if (this.CheckIfHitGround(frameTime, verticalVelocity, anchorOffset, out _)) {
            verticalVelocity = 0f;
            this.CurrentMovement = horizontalVelocity != 0f ? WalkerMovement.Walking : WalkerMovement.Idle;
        }
        else {
            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            verticalVelocity = Math.Max(-this.PhysicsLoop.TerminalVelocity, verticalVelocity);
            this.CurrentMovement = WalkerMovement.Falling;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(this.CurrentState.FacingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleIdle(FrameTime frameTime, float anchorOffset) {
        var horizontalVelocity = 0f;
        var verticalVelocity = 0f;
        var facingDirection = this.CurrentState.FacingDirection;

        if (this.CurrentState.SecondsInState > this.PauseTime) {
            this.CurrentMovement = WalkerMovement.Walking;
            if (facingDirection == HorizontalDirection.Left) {
                facingDirection = HorizontalDirection.Right;
                horizontalVelocity = this.MaxHorizontalVelocity;
            }
            else {
                facingDirection = HorizontalDirection.Left;
                horizontalVelocity = -this.MaxHorizontalVelocity;
            }
        }
        else if (!this.CheckIfStillGrounded(anchorOffset, out _)) {
            this.CurrentMovement = WalkerMovement.Falling;
            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }
        else {
            this.CurrentMovement = WalkerMovement.Idle;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(facingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private ActorState HandleWalking(FrameTime frameTime, float anchorOffset) {
        var horizontalVelocity = this.GetHorizontalVelocity(this.CurrentState.FacingDirection);
        var verticalVelocity = 0f;
        var facingDirection = this.CurrentState.FacingDirection;
        // TODO: check for ledge when applicable
        if (this.CheckIfHitWall(frameTime, horizontalVelocity, true, anchorOffset)) {
            this.CurrentMovement = WalkerMovement.Idle;
            horizontalVelocity = 0f;
        }
        else if (!this.CheckIfStillGrounded(anchorOffset, out _)) {
            this.CurrentMovement = WalkerMovement.Falling;
            verticalVelocity += this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        }
        else {
            this.CurrentMovement = WalkerMovement.Walking;
        }

        var velocity = new Vector2(horizontalVelocity, verticalVelocity);
        this.ApplyVelocity(frameTime, velocity);
        return new ActorState(facingDirection, this.Transform.Position, velocity, this.GetSecondsInState(frameTime));
    }

    private void ResetAnimation() {
        if (this._spriteAnimator != null) {
            if (this.CurrentMovement != this._previousMovement) {
                var spriteAnimation = this.CurrentMovement switch {
                    WalkerMovement.Idle => this.IdleAnimationReference.PackagedAsset,
                    WalkerMovement.Walking => this.WalkingAnimationReference.PackagedAsset,
                    WalkerMovement.Falling => this.FallingAnimationReference.PackagedAsset,
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