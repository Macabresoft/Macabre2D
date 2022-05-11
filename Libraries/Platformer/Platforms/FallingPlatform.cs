namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// The possible states of a falling platform.
/// </summary>
public enum FallingPlatformState {
    None,
    Waiting,
    Falling,
    Respawning
}

/// <summary>
/// A platform which falls upon an actor walking on it. It will respawn after a set amount of time.
/// </summary>
public class FallingPlatform : MoverPlatform, IUpdateableEntity {
    private Vector2 _initialPosition;
    private IPlatformerPhysicsLoop _physicsLoop = PlatformerPhysicsLoop.Empty;
    private FallingPlatformState _state = FallingPlatformState.None;
    private float _timeInState;
    private float _timeToFall = 3f;
    private float _timeToRespawn = 3f;
    private float _velocity;
    private float _gravityMultiplier = 1f;

    /// <summary>
    /// Gets or sets the time to fall until this disappears.
    /// </summary>
    [DataMember]
    public float TimeToFall {
        get => this._timeToFall;
        set => this.Set(ref this._timeToFall, Math.Max(value, 1f));
    }

    /// <summary>
    /// Gets or sets the time until this will respawn after its fall has finished.
    /// </summary>
    [DataMember]
    public float TimeToRespawn {
        get => this._timeToRespawn;
        set => this.Set(ref this._timeToRespawn, Math.Max(value, 1f));
    }

    /// <summary>
    /// Gets or sets the time until falling after an actor walks on it.
    /// </summary>
    [DataMember]
    public float TimeUntilFall { get; set; }

    /// <summary>
    /// Gets or sets the gravity multiplier.
    /// </summary>
    [DataMember]
    public float GravityMultiplier {
        get => this._gravityMultiplier;
        set {
            if (value > 0f) {
                this._gravityMultiplier = value;
            }
            
            this.RaisePropertyChanged();
        }
    }

    private float Velocity {
        get => this._velocity;
        set => this._velocity = Math.Max(-this._physicsLoop.TerminalVelocity * this.GravityMultiplier, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._physicsLoop = this.Scene.GetLoop<IPlatformerPhysicsLoop>() ?? throw new NotSupportedException(nameof(this._physicsLoop));
        this._initialPosition = this.Transform.Position;
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, Framework.InputState inputState) {
        switch (this._state) {
            case FallingPlatformState.Waiting: {
                this._timeInState += (float)frameTime.SecondsPassed;

                if (this._timeInState >= this.TimeUntilFall) {
                    this._timeInState = 0f;
                    this._state = FallingPlatformState.Falling;
                }

                break;
            }
            case FallingPlatformState.Respawning: {
                this._timeInState += (float)frameTime.SecondsPassed;

                if (this._timeInState >= this.TimeToRespawn) {
                    this._timeInState = 0f;
                    this._state = FallingPlatformState.None;
                    this.SetWorldPosition(this._initialPosition);
                    this.Collider.Layers = this.ColliderLayers;
                }

                break;
            }
            case FallingPlatformState.Falling: {
                this._timeInState += (float)frameTime.SecondsPassed;
                this.Velocity += this._physicsLoop.Gravity.Value.Y * this.GravityMultiplier * (float)frameTime.SecondsPassed;
                this.Move(new Vector2(0f, this.Velocity * (float)frameTime.SecondsPassed));

                if (this._timeInState >= this._timeToFall) {
                    this._timeInState = 0f;
                    this._state = FallingPlatformState.Respawning;
                    this._velocity = 0f;
                    this.Collider.Layers = Layers.None;
                }

                break;
            }
            case FallingPlatformState.None:
            default: {
                if (this.Attached.Any()) {
                    this._state = this.TimeUntilFall > 0f ? FallingPlatformState.Waiting : FallingPlatformState.Falling;
                    this._velocity = 0f;
                    this._timeInState = 0f;
                }

                break;
            }
        }
    }
}