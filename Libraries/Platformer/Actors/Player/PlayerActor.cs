namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// An implementation of <see cref="IActor"/> for the player.
/// </summary>
public class PlayerActor : UpdateableEntity, IActor, IBoundable {
    private HorizontalDirection _movementDirection;
    private Vector2 _playerSize = Vector2.One;
    private LoopingSpriteAnimator? _spriteAnimator;
    private ActorState _currentState;
    private float _halfWidth = 0.5f;

    /// <inheritdoc />
    public BoundingArea BoundingArea { get; private set; }

    /// <summary>
    /// Gets the movement direction of the player.
    /// </summary>
    public HorizontalDirection MovementDirection {
        get => this._movementDirection;
        set {
            if (this.Set(ref this._movementDirection, value) && this._spriteAnimator != null) {
                this._spriteAnimator.RenderSettings.FlipHorizontal = this._movementDirection == HorizontalDirection.Left;
            }
        }
    }

    /// <summary>
    /// Gets the player size in world units.
    /// </summary>
    [DataMember]
    public Vector2 PlayerSize {
        get => this._playerSize;
        private set {
            if (this.Set(ref this._playerSize, value)) {
                var worldPosition = this.Transform.Position;
                this.BoundingArea = new BoundingArea(worldPosition, worldPosition + this._playerSize);
                this._halfWidth = this._playerSize.X * 0.5f;
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._spriteAnimator = this.GetOrAddChild<LoopingSpriteAnimator>();
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this._spriteAnimator == null) {
        }

        // TODO: check user input and collision status
    }

    /// <inheritdoc />
    public ActorState CurrentState {
        get => this._currentState;
        private set => this.Set(ref this._currentState, value);
    }
}