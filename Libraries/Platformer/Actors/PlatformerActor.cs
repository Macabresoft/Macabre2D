namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an actor, which is
/// </summary>
public interface IPlatformerActor : IBaseActor, IBoundable {
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
    private IPlatformerPhysicsLoop _physicsLoop = PlatformerPhysicsLoop.Empty;
    private IBaseActor _platform = Empty;
    private Vector2 _size = Vector2.One;

    /// <inheritdoc />
    public BoundingArea BoundingArea { get; private set; }


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
    }

    /// <summary>
    /// Applies velocity.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="velocity">The velocity.</param>
    protected void ApplyVelocity(FrameTime frameTime, Vector2 velocity) {
        this.LocalPosition += velocity * (float)frameTime.SecondsPassed;
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

        if (result) {
            this.TrySetPlatform(hit);
        }

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
            this.TrySetPlatform(hit);
            this.SetWorldPosition(new Vector2(this.Transform.Position.X, hit.ContactPoint.Y + this.HalfSize.Y));
        }

        return result;
    }

    /// <summary>
    /// Moves this actor with the platform beneath it.
    /// </summary>
    protected void MoveWithPlatform() {
        if (this._platform.CurrentState.Position != this._platform.PreviousState.Position &&
            this._platform is IEntity entity && entity.TryGetChild<SimplePhysicsBody>(out var body) &&
            body is { HasCollider: true, Collider: LineCollider collider } && collider.WorldPoints.Any()) {
            var newPosition = new Vector2(this.Transform.Position.X + this._platform.CurrentState.Position.X - this._platform.PreviousState.Position.X, collider.WorldPoints.First().Y + this.HalfSize.Y);
            this.SetWorldPosition(newPosition);
        }
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

    private void TrySetPlatform(RaycastHit hit) {
        if (hit.Collider?.Body is IEntity entity && entity.TryGetParentEntity<IBaseActor>(out var platform) && platform != null) {
            this._platform = platform;
        }
        else {
            this._platform = Empty;
        }
    }
}