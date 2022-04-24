namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A platform which falls upon an actor walking on it. It will respawn after a set amount of time.
/// </summary>
public class FallingPlatform : MoverPlatform, IUpdateableEntity {
    private Vector2 _initialPosition;
    private bool _isFalling;
    private bool _isRespawning;
    private IPlatformerPhysicsLoop _physicsLoop = PlatformerPhysicsLoop.Empty;
    private float _timeInState;
    private float _timeToFall = 3f;
    private float _timeToRespawn = 3f;
    private float _velocity;

    /// <summary>
    /// The time to fall until this disappears.
    /// </summary>
    [DataMember]
    public float TimeToFall {
        get => this._timeToFall;
        set => this.Set(ref this._timeToFall, Math.Max(value, 1f));
    }

    /// <summary>
    /// The time until this will respawn after its fall has finished.
    /// </summary>
    [DataMember]
    public float TimeToRespawn {
        get => this._timeToRespawn;
        set => this.Set(ref this._timeToRespawn, Math.Max(value, 1f));
    }

    private float Velocity {
        get => this._velocity;
        set => this._velocity = Math.Max(-this._physicsLoop.TerminalVelocity, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._physicsLoop = this.Scene.GetLoop<IPlatformerPhysicsLoop>() ?? throw new NotSupportedException(nameof(this._physicsLoop));
        this._initialPosition = this.Transform.Position;
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        if (this._isRespawning) {
            this._timeInState += (float)frameTime.SecondsPassed;

            if (this._timeInState >= this.TimeToRespawn) {
                this._timeInState = 0f;
                this._isRespawning = false;
                this.SetWorldPosition(this._initialPosition);
                this.Collider.Layers = this.ColliderLayers;
            }
        }
        else if (this._isFalling) {
            this._timeInState += (float)frameTime.SecondsPassed;
            this.Velocity += this._physicsLoop.Gravity.Value.Y * (float)frameTime.SecondsPassed;
            this.Move(new Vector2(0f, this.Velocity * (float)frameTime.SecondsPassed));

            if (this._timeInState >= this._timeToFall) {
                this._timeInState = 0f;
                this._isFalling = false;
                this._isRespawning = true;
                this._velocity = 0f;
                this.Collider.Layers = Layers.None;
            }
        }
        else if (this.Attached.Any()) {
            this._isFalling = true;
            this._velocity = 0f;
            this._timeInState = 0f;
        }
    }
}