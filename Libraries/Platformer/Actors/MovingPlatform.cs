namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a moving platform.
/// </summary>
public interface IMovingPlatform {
    /// <summary>
    /// Attaches the actor to this platform.
    /// </summary>
    /// <param name="actor">The actor to attach.</param>
    void Attach(IPlatformerActor actor);

    /// <summary>
    /// Detaches the actor from this platform.
    /// </summary>
    /// <param name="actor">The actor to detach.</param>
    void Detach(IPlatformerActor actor);
}

/// <summary>
/// A moving platform.
/// </summary>
[Category("Platform")]
public class MovingPlatform : SimplePhysicsBody, IMovingPlatform, IUpdateableEntity {
    private readonly HashSet<IPlatformerActor> _attachedActors = new();
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
    public void Attach(IPlatformerActor actor) {
        this._attachedActors.Add(actor);
    }

    /// <inheritdoc />
    public void Detach(IPlatformerActor actor) {
        this._attachedActors.Remove(actor);
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
    public void Update(FrameTime frameTime, InputState inputState) {
        if (this._timePaused < this.PauseTimeInSeconds) {
            this._timePaused += (float)frameTime.SecondsPassed;
        }
        else {
            var originalPosition = this.Transform.Position;
            var desiredPosition = this._isTravelingToEnd ? this._endPoint : this._startPoint;
            var velocity = (desiredPosition - this.LocalPosition).GetNormalized() * this.Velocity * (float)frameTime.SecondsPassed;

            if (Math.Abs(this.LocalPosition.X - desiredPosition.X) < Math.Abs(velocity.X) || Math.Abs(this.LocalPosition.Y - desiredPosition.Y) < Math.Abs(velocity.Y)) {
                this.SetWorldPosition(desiredPosition);
                this._isTravelingToEnd = !this._isTravelingToEnd;
                this._timePaused = 0f;
            }
            else {
                this.Move(velocity);
            }

            if (this._attachedActors.Any()) {
                // TODO: this can only hand polygon colliders that are flat. Expand for any collider and find the actual collision spot.
                var attachedMovement = this.Transform.Position - originalPosition;
                var polygonCollider = this.Collider as PolygonCollider;
                var adjustForY = polygonCollider != null && polygonCollider.WorldPoints.Any() && Math.Abs(this._startPoint.Y - this._endPoint.Y) > 0.001f;
                var settings = this.Settings;
                var platformPixelOffset = this.Transform.ToPixelSnappedValue(settings).Position.X - this.Transform.Position.X;

                foreach (var attached in this._attachedActors) {
                    attached.Move(attachedMovement);
                    if (settings.SnapToPixels) {
                        attached.SetWorldPosition(new Vector2(attached.Transform.ToPixelSnappedValue(settings).Position.X + platformPixelOffset, attached.Transform.Position.Y));
                    }
                    
                    if (adjustForY) {
                        var yValue = polygonCollider?.WorldPoints.Select(x => x.Y).Max() ?? attached.Transform.Position.Y;
                        attached.SetWorldPosition(new Vector2(attached.Transform.Position.X, yValue + attached.HalfSize.Y));
                    }
                }
            }
        }
    }

    private void ResetEndPoint() {
        this._endPoint = this._startPoint + this.DistanceToTravel;
    }
}