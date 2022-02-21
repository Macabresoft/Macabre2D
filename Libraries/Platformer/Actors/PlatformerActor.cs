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
    /// Gets the actor's size in world units.
    /// </summary>
    Vector2 ActorSize { get; }

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
public class PlatformerActor : UpdateableEntity, IPlatformerActor {
    private float _acceleration = 6f;
    private Vector2 _actorSize = Vector2.One;
    private float _airAccelerationMultiplier = 0.9f;
    private ActorState _currentState;
    private float _decelerationRate = 25f;
    private float _gravity = 39f;
    private float _halfWidth = 0.5f;
    private float _jumpVelocity = 8f;
    private float _maximumHorizontalVelocity = 7f;
    private float _minimumVelocity = 2f;
    private HorizontalDirection _movementDirection;
    private LoopingSpriteAnimator? _spriteAnimator;
    private float _terminalVelocity = 15f;

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
    public Vector2 ActorSize {
        get => this._actorSize;
        private set {
            if (this.Set(ref this._actorSize, value)) {
                var worldPosition = this.Transform.Position;
                this.BoundingArea = new BoundingArea(worldPosition, worldPosition + this._actorSize);
                this._halfWidth = this._actorSize.X * 0.5f;
            }
        }
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
        protected set => this.Set(ref this._currentState, value);
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
    [DataMember]
    public float TerminalVelocity {
        get => this._terminalVelocity;
        protected set => this.Set(ref this._terminalVelocity, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._spriteAnimator = this.GetOrAddChild<LoopingSpriteAnimator>();
    }

    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this._spriteAnimator != null) {
            // TODO: set animation based on the current state. Overrides of Update should handle setting the state.
        }
    }
}