﻿namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an actor, which is
/// </summary>
public interface IPlatformerActor : IBoundable, ITransformable {
    /// <summary>
    /// Gets the current state of this actor.
    /// </summary>
    ActorState CurrentState { get; }

    /// <summary>
    /// Gets a value that is half of <see cref="Size" /> for calculations.
    /// </summary>
    public Vector2 HalfSize { get; }

    /// <summary>
    /// Gets a value indicating whether or not this actor is on a platform.
    /// </summary>
    bool IsOnPlatform { get; }

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
[Category("Actor")]
public abstract class PlatformerActor : UpdateableEntity, IPlatformerActor {
    private ActorState _currentState;
    private IPlatformerPhysicsLoop _physicsLoop = PlatformerPhysicsLoop.Empty;
    private IAttachable? _platform;
    private ActorState _previousState;
    private Vector2 _size = Vector2.One;

    /// <inheritdoc />
    public bool IsOnPlatform => this._platform != null;

    /// <inheritdoc />
    public BoundingArea BoundingArea { get; private set; }

    /// <inheritdoc />
    public ActorState CurrentState {
        get => this._currentState;
        protected set => this.Set(ref this._currentState, value);
    }

    /// <inheritdoc />
    public Vector2 HalfSize { get; private set; } = new(0.5f);

    /// <inheritdoc />
    public ActorState PreviousState {
        get => this._previousState;
        protected set => this.Set(ref this._previousState, value);
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

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.ResetBoundingArea();
        this._physicsLoop = this.Scene.GetLoop<IPlatformerPhysicsLoop>() ?? throw new NotSupportedException(nameof(this._physicsLoop));
    }

    /// <summary>
    /// Applies velocity.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="velocity">The velocity.</param>
    protected void ApplyVelocity(FrameTime frameTime, Vector2 velocity) {
        this.Move(velocity * (float)frameTime.SecondsPassed);
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
        var anchorOffset = this.Size.X * this.Settings.InversePixelsPerUnit;

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsLoop.CeilingLayer,
            out var hit,
            new Vector2(-this.HalfSize.X + anchorOffset, 0f),
            new Vector2(this.HalfSize.X - anchorOffset, 0f)) && hit != RaycastHit.Empty;

        if (result) {
            this.SetWorldPosition(new Vector2(worldTransform.Position.X, hit.ContactPoint.Y - this.HalfSize.X));

            if (hit.Collider?.Body is IBlock block) {
                block.Hit();
            }
        }

        return result;
    }

    /// <summary>
    /// Checks if this has hit the ground.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="verticalVelocity">The vertical velocity.</param>
    /// <param name="groundEntity">The ground entity.</param>
    /// <returns>A value indicating whether or not this actor has hit the ground.</returns>
    protected bool CheckIfHitGround(FrameTime frameTime, float verticalVelocity, out IEntity? groundEntity) {
        var direction = new Vector2(0f, -1f);
        var distance = this.HalfSize.Y + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);
        var anchorOffset = this.Size.X * this.Settings.InversePixelsPerUnit;

        var result = this.TryRaycast(
            direction,
            distance,
            this._physicsLoop.GroundLayer,
            out var hit,
            new Vector2(-this.HalfSize.X + anchorOffset, 0f),
            new Vector2(this.HalfSize.X - anchorOffset, 0f)) && hit != RaycastHit.Empty;

        if (result) {
            this.TrySetPlatform(hit);
            groundEntity = hit.Collider?.Body;
            this.SetWorldPosition(new Vector2(this.Transform.Position.X, hit.ContactPoint.Y + this.HalfSize.Y));
        }
        else {
            groundEntity = null;
        }

        return result;
    }

    /// <summary>
    /// Checks if this has hit a wall.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="horizontalVelocity">The horizontal velocity.</param>
    /// <param name="applyVelocityToRaycast">A value indicating whether or not to apply velocity to the raycast.</param>
    /// <param name="wallEntity">The entity which is a parent of the wall collider hit.</param>
    /// <returns>A value indicating whether or not this has hit a wall.</returns>
    protected bool CheckIfHitWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast, out IEntity? wallEntity) {
        var anchorOffset = this.Size.Y * this.Settings.InversePixelsPerUnit;
        if (horizontalVelocity != 0f) {
            return this.RaycastWall(frameTime, horizontalVelocity, applyVelocityToRaycast, anchorOffset, out wallEntity);
        }

        return this.RaycastWall(frameTime, -1f, false, anchorOffset, out wallEntity) || this.RaycastWall(frameTime, 1f, false, anchorOffset, out wallEntity);
    }

    /// <summary>
    /// Checks if this is still grounded.
    /// </summary>
    /// <param name="facingDirection">The facing direction.</param>
    /// <returns>A value indicating whether or not this is still grounded.</returns>
    protected bool CheckIfLedgeAhead(HorizontalDirection facingDirection) {
        var direction = new Vector2(0f, -1f);
        var anchor = facingDirection == HorizontalDirection.Left ? new Vector2(-this.HalfSize.X, 0f) : new Vector2(this.HalfSize.X, 0f);

        return !this.TryRaycast(
            direction,
            this.HalfSize.Y,
            this._physicsLoop.GroundLayer,
            out _,
            anchor);
    }

    /// <summary>
    /// Checks if this is still grounded.
    /// </summary>
    /// <returns>A value indicating whether or not this is still grounded.</returns>
    protected bool CheckIfStillGrounded() {
        var direction = new Vector2(0f, -1f);
        var result = false;
        var hit = RaycastHit.Empty;

        if (this.IsOnPlatform && this.PreviousState.Position.Y > this.CurrentState.Position.Y) {
            result = this.TryRaycast(
                direction,
                this.HalfSize.Y,
                this._physicsLoop.GroundLayer,
                out hit,
                new Vector2(-this.HalfSize.X, 0f),
                new Vector2(this.HalfSize.X, 0f)) && hit != RaycastHit.Empty;

            if (result) {
                this.UnsetPlatform();
            }
        }

        if (!result) {
            result = this.CheckIfStillOnPlatform(out hit);

            if (!result) {
                result = this.TryRaycast(
                    direction,
                    this.HalfSize.Y,
                    this._physicsLoop.GroundLayer,
                    out hit,
                    new Vector2(-this.HalfSize.X, 0f),
                    new Vector2(this.HalfSize.X, 0f)) && hit != RaycastHit.Empty;
            }
        }

        if (result) {
            this.TrySetPlatform(hit);
            this.SetWorldPosition(new Vector2(this.Transform.Position.X, hit.ContactPoint.Y + this.HalfSize.Y));
        }
        else {
            this.UnsetPlatform();
        }

        return result;
    }

    /// <summary>
    /// Unsets the platform.
    /// </summary>
    protected void UnsetPlatform() {
        if (this._platform != null) {
            this._platform.Detach(this);
            this._platform = null;
        }
    }

    private bool CheckIfStillOnPlatform(out RaycastHit hit) {
        var direction = new Vector2(0f, -1f);
        var result = false;
        hit = RaycastHit.Empty;

        if (this.IsOnPlatform) {
            if (this.TryRaycastAll(
                    direction,
                    this.HalfSize.Y + this.Settings.InversePixelsPerUnit,
                    this._physicsLoop.GroundLayer,
                    out var hits,
                    new Vector2(-this.HalfSize.X, 0f),
                    new Vector2(this.HalfSize.X, 0f))) {
                hit = hits.FirstOrDefault(x => x.Collider?.Body == this._platform) ?? RaycastHit.Empty;
                result = hit != RaycastHit.Empty;
            }
        }

        return result;
    }

    private bool RaycastWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast, float anchorOffset, out IEntity? wallEntity) {
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
            wallEntity = hit.Collider?.Body;
        }
        else {
            wallEntity = null;
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

        var position = this.Transform.Position;
        var counter = 0;

        while (!result && counter < anchors.Length) {
            var (x, y) = anchors[counter];
            result = this._physicsLoop.TryRaycast(new Vector2(position.X + x, position.Y + y), direction, distance, layers, out hit);
            counter++;
        }

        return result;
    }

    private bool TryRaycastAll(Vector2 direction, float distance, Layers layers, out IEnumerable<RaycastHit> hits, params Vector2[] anchors) {
        var result = false;
        var actualHits = new List<RaycastHit>();

        var position = this.Transform.Position;

        foreach (var anchor in anchors) {
            var (x, y) = anchor;
            var potentialHits = this._physicsLoop.RaycastAll(new Vector2(position.X + x, position.Y + y), direction, distance, layers);
            if (potentialHits.Any()) {
                actualHits.AddRange(potentialHits);
                result = true;
            }
        }

        hits = actualHits;
        return result;
    }

    private void TrySetPlatform(RaycastHit hit) {
        if (hit.Collider?.Body is IAttachable platform) {
            if (platform != this._platform) {
                this._platform?.Detach(this);
                this._platform = platform;
                this._platform.Attach(this);
            }
        }
    }
}