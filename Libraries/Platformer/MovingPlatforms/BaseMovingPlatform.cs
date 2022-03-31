namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

public class BaseMovingPlatform : SimplePhysicsBody, IMovingPlatform {
    private readonly HashSet<IPlatformerActor> _attached = new();
    private Vector2 _previousPosition;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._previousPosition = this.Transform.Position;
    }

    /// <inheritdoc />
    public void Attach(IPlatformerActor actor) {
        this._attached.Add(actor);
    }

    /// <inheritdoc />
    public void Detach(IPlatformerActor actor) {
        this._attached.Remove(actor);
    }
    
    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.Transform)) {
            this.MoveAttached(this.Transform.Position - this._previousPosition);
            this._previousPosition = this.Transform.Position;
        }
    }
    
    /// <summary>
    /// Moves the attached actors.
    /// </summary>
    /// <param name="amount">The amount to move attached actors.</param>
    protected void MoveAttached(Vector2 amount) {
        if (this._attached.Any()) {
            // TODO: this can only hand polygon colliders that are flat. Expand for any collider and find the actual collision spot.
            var polygonCollider = this.Collider as PolygonCollider;
            var adjustForY = amount.Y != 0f && polygonCollider != null && polygonCollider.WorldPoints.Any();
            var adjustForPixels = this.Settings.SnapToPixels && this.Transform.Position != this._previousPosition && amount.X != 0f;
            var settings = this.Settings;
            var platformPixelOffset = this.Transform.ToPixelSnappedValue(settings).Position.X - this.Transform.Position.X;

            foreach (var attached in this._attached) {
                attached.Move(amount);
                if (adjustForPixels && attached.CurrentState.Velocity.X == 0f) {
                    attached.SetWorldPosition(new Vector2(attached.Transform.ToPixelSnappedValue(settings).Position.X - platformPixelOffset, attached.Transform.Position.Y));
                }

                if (adjustForY) {
                    var yValue = polygonCollider?.WorldPoints.Select(x => x.Y).Max() ?? attached.Transform.Position.Y;
                    attached.SetWorldPosition(new Vector2(attached.Transform.Position.X, yValue + attached.HalfSize.Y));
                }
            }
        }
    }
}