﻿namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for an actor, which is
/// </summary>
public interface IPlatformerActor : IBaseActor, IBoundable {
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
    /// Gets the moving animation reference.
    /// </summary>
    SpriteAnimationReference MovingAnimationReference { get; }

    /// <summary>
    /// Gets the actor's size in world units.
    /// </summary>
    Vector2 Size { get; }
}

/// <summary>
/// An actor that moves and animates with a platformer focus.
/// </summary>
[Category("Actor")]
public abstract class PlatformerActor : BaseActor, IPlatformerActor {
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;
    private IPlatformerPhysicsLoop _physicsLoop = PlatformerPhysicsLoop.Empty;
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
    public BoundingArea BoundingArea { get; private set; }


    /// <inheritdoc />
    [DataMember]
    [Category("Movement")]
    public float JumpVelocity {
        get => this._jumpVelocity;
        protected set => this.Set(ref this._jumpVelocity, value);
    }

    /// <inheritdoc />
    [DataMember]
    [Category("Movement")]
    public float MaximumHorizontalVelocity {
        get => this._maximumHorizontalVelocity;
        protected set => this.Set(ref this._maximumHorizontalVelocity, value);
    }


    /// <inheritdoc />
    [DataMember]
    public Vector2 Size {
        get => this._size;
        set {
            if (this.Set(ref this._size, value)) {
                this.ResetBoundingArea();
            }
        }
    }

    /// <summary>
    /// Gets the physics system.
    /// </summary>
    protected IPlatformerPhysicsLoop PhysicsLoop => this._physicsLoop;

    /// <summary>
    /// Gets a value that is half of <see cref="Size" /> for calculations.
    /// </summary>
    protected Vector2 HalfSize { get; private set; } = new(0.5f);

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.ResetBoundingArea();
        this._physicsLoop = this.Scene.GetLoop<IPlatformerPhysicsLoop>() ?? throw new ArgumentNullException(nameof(this._physicsLoop));
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.IdleAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.MovingAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.JumpingAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.FallingAnimationReference);
        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

        if (this.IdleAnimationReference.PackagedAsset is { } animation) {
            this._spriteAnimator.Play(animation, true);
        }
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        var anchorOffset = this._size.Y / 8;
        this.PreviousState = this.CurrentState;
        var currentState = this.GetNewActorState(frameTime, anchorOffset);

        if (this._spriteAnimator != null) {
            if (currentState.MovementKind != this.PreviousState.MovementKind) {
                var spriteAnimation = currentState.MovementKind switch {
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

            this._spriteAnimator.RenderSettings.FlipHorizontal = currentState.FacingDirection == HorizontalDirection.Left;
        }

        this.ApplyVelocity(frameTime, currentState.Velocity);
        this.CurrentState = new ActorState(currentState.MovementKind, currentState.FacingDirection, this.Transform.Position, currentState.Velocity, currentState.SecondsInState);
    }

    /// <summary>
    /// Checks if this has hit a ceiling.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="verticalVelocity">The vertical velocity.</param>
    /// <param name="anchorOffset">The anchor offset for raycasting.</param>
    /// <returns>A value indicating whether or not this has hit a ceiling.</returns>
    protected bool CheckIfHitCeiling(FrameTime frameTime, float verticalVelocity, float anchorOffset) {
        var worldTransform = this.Transform;
        var direction = new Vector2(0f, 1f);
        var distance = this.HalfSize.Y + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsLoop.CeilingLayer,
            out var hit,
            new Vector2(-this.HalfSize.X + anchorOffset, 0f),
            new Vector2(this.HalfSize.X - anchorOffset, 0f)) && hit != RaycastHit.Empty;

        if (result) {
            this.SetWorldPosition(new Vector2(worldTransform.Position.X, hit.ContactPoint.Y - this.HalfSize.X));
        }

        return result;
    }

    /// <summary>
    /// Checks if this has hit the ground.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="verticalVelocity">The vertical velocity.</param>
    /// <param name="anchorOffset">The anchor offset for raycasting.</param>
    /// <param name="hit">The raycast hit.</param>
    /// <returns>A value indicating whether or not this actor has hit the ground.</returns>
    protected bool CheckIfHitGround(FrameTime frameTime, float verticalVelocity, float anchorOffset, out RaycastHit hit) {
        var direction = new Vector2(0f, -1f);
        var distance = this.HalfSize.Y + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsLoop.GroundLayer,
            out hit,
            new Vector2(-this.HalfSize.X + anchorOffset, 0f),
            new Vector2(this.HalfSize.X - anchorOffset, 0f)) && hit != RaycastHit.Empty;

        return result;
    }

    /// <summary>
    /// Checks if this has hit a wall.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="horizontalVelocity">The horizontal velocity.</param>
    /// <param name="applyVelocityToRaycast">A value indicating whether or not to apply velocity to the raycast.</param>
    /// <param name="anchorOffset">The anchor offset.</param>
    /// <returns>A value indicating whether or not this has hit a wall.</returns>
    protected bool CheckIfHitWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast, float anchorOffset) {
        if (horizontalVelocity != 0f) {
            return this.RaycastWall(frameTime, horizontalVelocity, applyVelocityToRaycast, anchorOffset);
        }

        return this.RaycastWall(frameTime, -1f, false, anchorOffset) || this.RaycastWall(frameTime, 1f, false, anchorOffset);
    }

    protected bool CheckIfStillGrounded(float anchorOffset, out RaycastHit hit) {
        var direction = new Vector2(0f, -1f);

        var result = this.TryRaycast(
            direction,
            this.HalfSize.Y,
            this._physicsLoop.GroundLayer,
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
    /// <param name="facingDirection">The direction this is facing.</param>
    /// <param name="secondsInState">The number of seconds this has been in a falling state.</param>
    /// <returns>The actor's state.</returns>
    protected ActorState GetFallingState(FrameTime frameTime, float horizontalVelocity, HorizontalDirection facingDirection, float secondsInState) {
        var verticalVelocity = -this.PhysicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
        return new ActorState(MovementKind.Falling, facingDirection, this.Transform.Position, new Vector2(horizontalVelocity, verticalVelocity), secondsInState);
    }

    /// <summary>
    /// Gets the jumping state for a newly jumping actor.
    /// </summary>
    /// <param name="horizontalVelocity">The horizontal velocity.</param>
    /// <param name="facingDirection">The direction this is facing.</param>
    /// <param name="secondsInState">The number of seconds this has been in a falling state.</param>
    /// <returns>The actor's state.</returns>
    protected ActorState GetJumpingState(float horizontalVelocity, HorizontalDirection facingDirection, float secondsInState) {
        return new ActorState(MovementKind.Jumping, facingDirection, this.Transform.Position, new Vector2(horizontalVelocity, this.JumpVelocity), secondsInState);
    }

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Falling" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="anchorOffset">The anchor offset.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleFalling(FrameTime frameTime, float anchorOffset);

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Idle" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="anchorOffset">The anchor offset.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleIdle(FrameTime frameTime, float anchorOffset);

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Jumping" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="anchorOffset">The anchor offset.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleJumping(FrameTime frameTime, float anchorOffset);

    /// <summary>
    /// Handles interactions during the <see cref="MovementKind.Moving" /> movement state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="anchorOffset">The anchor offset.</param>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState HandleMoving(FrameTime frameTime, float anchorOffset);


    private void ApplyVelocity(FrameTime frameTime, Vector2 velocity) {
        this.LocalPosition += velocity * (float)frameTime.SecondsPassed;
    }

    private ActorState GetNewActorState(FrameTime frameTime, float anchorOffset) {
        if (this.Size.X > 0f && this.Size.Y > 0f) {
            return this.CurrentState.MovementKind switch {
                MovementKind.Idle => this.HandleIdle(frameTime, anchorOffset),
                MovementKind.Moving => this.HandleMoving(frameTime, anchorOffset),
                MovementKind.Jumping => this.HandleJumping(frameTime, anchorOffset),
                MovementKind.Falling => this.HandleFalling(frameTime, anchorOffset),
                _ => this.CurrentState
            };
        }

        return this.CurrentState;
    }

    private bool RaycastWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast, float anchorOffset) {
        var transform = this.Transform;
        var isDirectionPositive = horizontalVelocity >= 0f;
        var direction = new Vector2(isDirectionPositive ? 1f : -1f, 0f);
        var distance = applyVelocityToRaycast ? this.HalfSize.X + (float)Math.Abs(horizontalVelocity * frameTime.SecondsPassed) : this.HalfSize.X;

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsLoop.WallLayer,
            out var hit,
            new Vector2(0f, -this.HalfSize.Y + anchorOffset),
            new Vector2(0f, this.HalfSize.Y - anchorOffset)) && hit != RaycastHit.Empty;

        if (result) {
            this.SetWorldPosition(new Vector2(hit.ContactPoint.X + (isDirectionPositive ? -this.HalfSize.X : this.HalfSize.X), transform.Position.Y));
        }

        return result;
    }

    private void ResetBoundingArea() {
        var worldPosition = this.Transform.Position;
        this.HalfSize = this._size * 0.5f;
        this.BoundingArea = new BoundingArea(worldPosition - this.HalfSize, worldPosition + this.HalfSize);
    }

    private bool TryRaycast(Vector2 direction, float distance, Layers layers, out RaycastHit hit, params Vector2[] anchors) {
        var result = false;
        hit = RaycastHit.Empty;

        var worldTransform = this.Transform;
        var counter = 0;

        while (!result && counter < anchors.Length) {
            var (x, y) = anchors[counter];
            result = this._physicsLoop.TryRaycast(new Vector2(worldTransform.Position.X + x, worldTransform.Position.Y + y), direction, distance, layers, out hit);
            counter++;
        }

        return result;
    }
}