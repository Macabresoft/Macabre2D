﻿namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Libraries.Platformer.Physics;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an actor, which is
/// </summary>
public interface IPlatformerActor : IBoundable {
    /// <summary>
    /// Gets the acceleration of this actor. This is the rate at which it gains speed when intentionally moving in a direction.
    /// </summary>
    float Acceleration { get; }

    /// <summary>
    /// Gets a multiplier for air acceleration. By making this value less than one, the player will have less control in the air than they do when grounded.
    /// </summary>
    float AirAccelerationMultiplier { get; }

    /// <summary>
    /// Gets the current state of this actor.
    /// </summary>
    ActorState CurrentState { get; }

    /// <summary>
    /// Gets the rate at which this actor decelerates in units per second.
    /// </summary>
    float DecelerationRate { get; }

    /// <summary>
    /// Gets the falling animation reference.
    /// </summary>
    SpriteAnimationReference FallingAnimationReference { get; }

    /// <summary>
    /// Gets the idle animation reference.
    /// </summary>
    public SpriteAnimationReference IdleAnimationReference { get; }

    /// <summary>
    /// Gets the jumping animation reference.
    /// </summary>
    SpriteAnimationReference JumpingAnimationReference { get; }

    /// <summary>
    /// Gets the initial velocity of a jump for this actor.
    /// </summary>
    float JumpVelocity { get; }

    /// <summary>
    /// Gets the maximum horizontal velocity allowed.
    /// </summary>
    float MaximumHorizontalVelocity { get; }

    /// <summary>
    /// Gets the minimum velocity before this actor will stop moving (when no force is causing it to move).
    /// </summary>
    float MinimumVelocity { get; }

    /// <summary>
    /// Gets the current movement direction.
    /// </summary>
    HorizontalDirection MovementDirection { get; }

    /// <summary>
    /// Gets the moving animation reference.
    /// </summary>
    SpriteAnimationReference MovingAnimationReference { get; }

    /// <summary>
    /// Gets the previous state of this actor.
    /// </summary>
    ActorState PreviousState { get; }

    /// <summary>
    /// Gets the actor's size in world units.
    /// </summary>
    Vector2 Size { get; }
}

/// <summary>
/// An actor that moves and animates with a platformer focus.
/// </summary>
public abstract class PlatformerActor : UpdateableEntity, IPlatformerActor {
    private float _acceleration = 6f;
    private float _airAccelerationMultiplier = 0.9f;
    private ActorState _currentState;
    private float _decelerationRate = 25f;
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;
    private float _minimumVelocity = 2f;
    private HorizontalDirection _movementDirection;
    private IPlatformerPhysicsSystem _physicsSystem = PlatformerPhysicsSystem.Empty;
    private ActorState _previousState;
    private Vector2 _size = Vector2.One;
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

    /// <inheritdoc />
    [DataMember]
    public float Acceleration {
        get => this._acceleration;
        protected set => this.Set(ref this._acceleration, value);
    }

    /// <inheritdoc />
    [DataMember]
    public float AirAccelerationMultiplier {
        get => this._airAccelerationMultiplier;
        protected set => this.Set(ref this._airAccelerationMultiplier, value);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea { get; private set; }


    /// <inheritdoc />
    public ActorState CurrentState {
        get => this._currentState;
        private set => this.Set(ref this._currentState, value);
    }

    /// <inheritdoc />
    [DataMember]
    public float DecelerationRate {
        get => this._decelerationRate;
        protected set => this.Set(ref this._decelerationRate, value);
    }

    /// <inheritdoc />
    [DataMember]
    public float JumpVelocity {
        get => this._jumpVelocity;
        protected set => this.Set(ref this._jumpVelocity, value);
    }

    /// <inheritdoc />
    [DataMember]
    public float MaximumHorizontalVelocity {
        get => this._maximumHorizontalVelocity;
        protected set => this.Set(ref this._maximumHorizontalVelocity, value);
    }

    /// <inheritdoc />
    [DataMember]
    public float MinimumVelocity {
        get => this._minimumVelocity;
        protected set => this.Set(ref this._minimumVelocity, value);
    }

    /// <summary>
    /// Gets the movement direction of the player.
    /// </summary>
    public HorizontalDirection MovementDirection {
        get => this._movementDirection;
        protected set {
            if (this.Set(ref this._movementDirection, value) && this._spriteAnimator != null) {
                this._spriteAnimator.RenderSettings.FlipHorizontal = this._movementDirection == HorizontalDirection.Left;
            }
        }
    }

    /// <inheritdoc />
    public ActorState PreviousState {
        get => this._previousState;
        private set => this.Set(ref this._previousState, value);
    }

    /// <inheritdoc />
    [DataMember]
    public Vector2 Size {
        get => this._size;
        private set {
            if (this.Set(ref this._size, value)) {
                var worldPosition = this.Transform.Position;
                this.BoundingArea = new BoundingArea(worldPosition, worldPosition + this._size);
                this.HalfSize = this._size * 0.5f;
            }
        }
    }

    /// <summary>
    /// Gets the physics system.
    /// </summary>
    protected IPlatformerPhysicsSystem PhysicsSystem => this._physicsSystem;

    /// <summary>
    /// Gets a value that is half of <see cref="Size" /> for calculations.
    /// </summary>
    protected Vector2 HalfSize { get; private set; } = new(0.5f);

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._physicsSystem = this.Scene.GetSystem<IPlatformerPhysicsSystem>() ?? throw new ArgumentNullException(nameof(this._physicsSystem));
        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this.PreviousState = this._currentState;
        this.CurrentState = this.GetNewActorState(frameTime, inputState);

        if (this._spriteAnimator != null && this._currentState.MovementKind != this._previousState.MovementKind) {
            var spriteAnimation = this._currentState.MovementKind switch {
                MovementKind.Idle => this.IdleAnimationReference.PackagedAsset,
                MovementKind.Moving => this.MovingAnimationReference.PackagedAsset,
                MovementKind.Falling when this.CurrentState.Velocity.Y < 0f => this.FallingAnimationReference.PackagedAsset,
                MovementKind.Jumping or MovementKind.Falling => this.JumpingAnimationReference.PackagedAsset,
                _ => null
            };

            if (spriteAnimation != null) {
                this._spriteAnimator.Play(spriteAnimation, true);
            }
        }
    }

    /// <summary>
    /// Checks if this has hit a ceiling.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="verticalVelocity">The vertical velocity.</param>
    /// <returns>A value indicating whether or not this has hit a ceiling.</returns>
    protected bool CheckIfHitCeiling(FrameTime frameTime, float verticalVelocity) {
        var worldTransform = this.Transform;
        var direction = new Vector2(0f, 1f);
        var distance = this.HalfSize.Y + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsSystem.CeilingLayer,
            out var hit,
            new Vector2(-this.HalfSize.X, 0f),
            new Vector2(this.HalfSize.X, 0f)) && hit != RaycastHit.Empty;

        if (result) {
            this.SetWorldPosition(new Vector2(worldTransform.Position.X, hit.ContactPoint.Y - this.HalfSize.X));
        }

        return result;
    }

    /// <summary>
    /// Checks if this has hit the ground.
    /// </summary>
    /// <param name="frameTime"></param>
    /// <param name="verticalVelocity"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    protected bool CheckIfHitGround(FrameTime frameTime, float verticalVelocity, out RaycastHit hit) {
        var direction = new Vector2(0f, -1f);
        var distance = this.HalfSize.Y + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsSystem.GroundLayer,
            out hit,
            new Vector2(-this.HalfSize.X, 0f),
            new Vector2(this.HalfSize.X, 0f)) && hit != RaycastHit.Empty;

        return result;
    }

    /// <summary>
    /// Checks if this has hit a wall.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="horizontalVelocity">The horizontal velocity.</param>
    /// <param name="applyVelocityToRaycast">A value indicating whether or not to apply velocity to the raycast.</param>
    /// <returns>A value indicating whether or not this has hit a wall.</returns>
    protected bool CheckIfHitWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast) {
        if (horizontalVelocity != 0f) {
            return this.RaycastWall(frameTime, horizontalVelocity, applyVelocityToRaycast);
        }

        return this.RaycastWall(frameTime, -1f, false) || this.RaycastWall(frameTime, 1f, false);
    }

    protected bool CheckIfStillGrounded(out RaycastHit hit) {
        var direction = new Vector2(0f, -1f);

        var result = this.TryRaycast(
            direction,
            this.HalfSize.Y,
            this._physicsSystem.GroundLayer,
            out hit,
            new Vector2(-this.HalfSize.X, 0f),
            new Vector2(this.HalfSize.X, 0f)) && hit != RaycastHit.Empty;

        if (result) {
            this.SetWorldPosition(new Vector2(this.Transform.Position.X, hit.ContactPoint.Y + this.HalfSize.Y));
        }

        return result;
    }

    /// <summary>
    /// Gets the falling state for a newly falling actor.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="horizontalVelocity">The horizontal velocity.</param>
    /// <returns>The actor's state.</returns>
    protected ActorState GetFallingState(FrameTime frameTime, float horizontalVelocity) {
        var verticalVelocity = -this.PhysicsSystem.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        return new ActorState(MovementKind.Falling, this.Transform.Position, new Vector2(horizontalVelocity, verticalVelocity));
    }

    /// <summary>
    /// Gets the horizontal acceleration of this actor.
    /// </summary>
    /// <returns>The horizontal acceleration.</returns>
    protected float GetHorizontalAcceleration() {
        return this.CurrentState.MovementKind == MovementKind.Moving ? this.Acceleration : this.Acceleration * this.AirAccelerationMultiplier;
    }

    /// <summary>
    /// Gets the jumping state for a newly jumping actor.
    /// </summary>
    /// <param name="horizontalVelocity">The horizontal velocity.</param>
    /// <returns>The actor's state.</returns>
    protected ActorState GetJumpingState(float horizontalVelocity) {
        return new ActorState(MovementKind.Jumping, this.Transform.Position, new Vector2(horizontalVelocity, this.JumpVelocity));
    }

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Falling" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleFalling(FrameTime frameTime, InputState inputState);

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Idle" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleIdle(FrameTime frameTime, InputState inputState);

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Jumping" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleJumping(FrameTime frameTime, InputState inputState);

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Moving" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleMoving(FrameTime frameTime, InputState inputState);
    
    private ActorState GetNewActorState(FrameTime frameTime, InputState inputState) {
        if (this.Size.X > 0f && this.Size.Y > 0f) {
            return this.CurrentState.MovementKind switch {
                MovementKind.Idle => this.HandleIdle(frameTime, inputState),
                MovementKind.Moving => this.HandleMoving(frameTime, inputState),
                MovementKind.Jumping => this.HandleJumping(frameTime, inputState),
                MovementKind.Falling => this.HandleFalling(frameTime, inputState),
                _ => this.CurrentState
            };
        }

        return this.CurrentState;
    }

    private bool RaycastWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast) {
        var transform = this.Transform;
        var isDirectionPositive = horizontalVelocity >= 0f;
        var direction = new Vector2(isDirectionPositive ? 1f : -1f, 0f);
        var distance = applyVelocityToRaycast ? this.HalfSize.X + (float)Math.Abs(horizontalVelocity * frameTime.SecondsPassed) : this.HalfSize.X;

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsSystem.WallLayer,
            out var hit,
            new Vector2(0f, -this.HalfSize.Y),
            new Vector2(0f, this.HalfSize.Y)) && hit != RaycastHit.Empty;

        if (result) {
            this.SetWorldPosition(new Vector2(hit.ContactPoint.X + (isDirectionPositive ? -this.HalfSize.X : this.HalfSize.X), transform.Position.Y));
        }

        return result;
    }

    private bool TryRaycast(Vector2 direction, float distance, Layers layers, out RaycastHit hit, params Vector2[] anchors) {
        var result = false;
        hit = RaycastHit.Empty;

        var worldTransform = this.Transform;
        var counter = 0;

        while (!result && counter < anchors.Length) {
            var (x, y) = anchors[counter];
            result = this._physicsSystem.TryRaycast(new Vector2(worldTransform.Position.X + x, worldTransform.Position.Y + y), direction, distance, layers, out hit);
            counter++;
        }

        return result;
    }
}