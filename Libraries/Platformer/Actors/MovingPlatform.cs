namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A moving platform.
/// </summary>
[Category("Platform")]
public class MovingPlatform : BaseActor {
    private Vector2 _distanceToTravel;
    private Vector2 _endPoint;
    private bool _isTravelingToEnd = true;
    private float _pauseTimeInSeconds;
    private Vector2 _startPoint;
    private float _timePaused;
    private float _velocity;

    /// <summary>
    /// Gets or sets the distance to travel. This can be considered the end point of the platformer in local units.
    /// </summary>
    [DataMember]
    public Vector2 DistanceToTravel {
        get => this._distanceToTravel;
        set {
            if (this.Set(ref this._distanceToTravel, value)) {
                this.ResetEndPoint();
            }
        }
    }

    /// <summary>
    /// Gets or sets the pause time in seconds.
    /// </summary>
    [DataMember]
    public float PauseTimeInSeconds {
        get => this._pauseTimeInSeconds;
        set => this.Set(ref this._pauseTimeInSeconds, value);
    }


    /// <summary>
    /// Gets or sets the velocity.
    /// </summary>
    [DataMember]
    public float Velocity {
        get => this._velocity;
        set => this.Set(ref this._velocity, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.GetOrAddChild<SimplePhysicsBody>();
        this._timePaused = this.PauseTimeInSeconds;
        this._startPoint = this.LocalPosition;
        this.ResetEndPoint();
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this.PreviousState = this.CurrentState;
        if (this._timePaused < this.PauseTimeInSeconds) {
            this._timePaused += (float)frameTime.SecondsPassed;
            this.CurrentState = new ActorState(HorizontalDirection.Right, this.Transform.Position, Vector2.Zero, this._timePaused);
        }
        else {
            var desiredPosition = this._isTravelingToEnd ? this._endPoint : this._startPoint;
            var velocity = (desiredPosition - this.LocalPosition).GetNormalized() * this.Velocity * (float)frameTime.SecondsPassed;

            if (Math.Abs(this.LocalPosition.X - desiredPosition.X) < Math.Abs(velocity.X) || Math.Abs(this.LocalPosition.Y - desiredPosition.Y) < Math.Abs(velocity.Y)) {
                this.LocalPosition = desiredPosition;
                this._isTravelingToEnd = !this._isTravelingToEnd;
                this._timePaused = 0f;
            }
            else {
                this.LocalPosition += velocity;
            }

            this.CurrentState = new ActorState(HorizontalDirection.Right, this.Transform.Position, velocity, 0f);
        }
    }

    private void ResetEndPoint() {
        this._endPoint = this._startPoint + this.DistanceToTravel;
    }
}