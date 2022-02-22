namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
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
    /// Gets the gravity for this actor (or alternatively, the rate at which it will accelerate downwards when falling in units per second).
    /// </summary>
    float Gravity { get; }

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

    /// <summary>
    /// Gets the maximum downward velocity when falling.
    /// </summary>
    float TerminalVelocity { get; }

    /// <summary>
    /// Gets the horizontal acceleration of this actor.
    /// </summary>
    /// <returns>The horizontal acceleration.</returns>
    float GetHorizontalAcceleration() {
        return this.CurrentState.MovementKind == MovementKind.Moving ? this.Acceleration : this.Acceleration * this.AirAccelerationMultiplier;
    }
}

/// <summary>
/// An actor that moves and animates with a platformer focus.
/// </summary>
public abstract class PlatformerActor : UpdateableEntity, IPlatformerActor {
    private float _acceleration = 6f;
    private Vector2 _size = Vector2.One;
    private float _airAccelerationMultiplier = 0.9f;
    private ActorState _currentState;
    private float _decelerationRate = 25f;
    private float _gravity = 39f;
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;
    private float _minimumVelocity = 2f;
    private HorizontalDirection _movementDirection;
    private ActorState _previousState;
    private QueueableSpriteAnimator? _spriteAnimator;
    private float _terminalVelocity = 15f;
    private ISimplePhysicsSystem? _physicsSystem;

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
    public float Gravity {
        get => this._gravity;
        protected set => this.Set(ref this._gravity, value);
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
    /// Gets a value that is half of <see cref="Size"/> for calculations.
    /// </summary>
    protected Vector2 HalfSize { get; private set; } = new(0.5f);

    /// <inheritdoc />
    [DataMember]
    public float TerminalVelocity {
        get => this._terminalVelocity;
        protected set => this.Set(ref this._terminalVelocity, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._physicsSystem = this.Scene.GetSystem<ISimplePhysicsSystem>();
        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this.PreviousState = this._currentState;
        this.CurrentState = this.GetNewActorState();

        if (this._spriteAnimator != null && this._currentState.MovementKind != this._previousState.MovementKind) {
            var spriteAnimation = this._currentState.MovementKind switch {
                MovementKind.Idle => this.IdleAnimationReference.PackagedAsset,
                MovementKind.Moving => this.MovingAnimationReference.PackagedAsset,
                MovementKind.Jumping => this.JumpingAnimationReference.PackagedAsset,
                MovementKind.Falling => this.FallingAnimationReference.PackagedAsset,
                _ => null
            };

            if (spriteAnimation != null) {
                this._spriteAnimator.Play(spriteAnimation, true);
            }
        }
    }
    
    private bool CheckIfHitCeiling(FrameTime frameTime, float verticalVelocity) {
        var worldTransform = this.Transform;
        var direction = new Vector2(0f, 1f);
        var distance = PlayerMovementValues.PlayerHalfWidth + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);

        var result = this.TryRaycast(
            direction,
            distance,
            this.PlayerComponent.CeilingLayer,
            out var hit,
            new Vector2(-AnchorValue, 0f),
            new Vector2(AnchorValue, 0f));

        if (result && hit != RaycastHit.Empty) {
            this.SetWorldPosition(new Vector2(worldTransform.Position.X, hit.ContactPoint.Y - this.HalfSize.X));
        }

        return result;
    }
    
    internal bool TryRaycast(Vector2 direction, float distance, Layers layers, out RaycastHit hit, params Vector2[] anchors) {
        var result = false;
        hit = RaycastHit.Empty;

        if (this._physicsSystem != null) {
            var worldTransform = this.Transform;
            var counter = 0;

            while (!result && counter < anchors.Length) {
                var (x, y) = anchors[counter];
                result = this._physicsSystem.TryRaycast(new Vector2(worldTransform.Position.X + x, worldTransform.Position.Y + y), direction, distance, layers, out hit);
                counter++;
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the new <see cref="ActorState" /> based on inheritor's preferences.
    /// </summary>
    /// <returns>A new actor state.</returns>
    protected abstract ActorState GetNewActorState();
}