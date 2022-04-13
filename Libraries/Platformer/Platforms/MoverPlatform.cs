namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A base class for a <see cref="IPlatform" /> that moves actors each frame.
/// </summary>
public class MoverPlatform : Platform {
    private Vector2 _previousPosition;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._previousPosition = this.Transform.Position;
    }

    /// <summary>
    /// Moves the attached actors.
    /// </summary>
    /// <param name="amount">The amount to move attached actors.</param>
    protected void MoveAttached(Vector2 amount) {
        if (this.Attached.Any()) {
            var polygonCollider = this.Collider as PolygonCollider;
            var adjustForY = amount.Y != 0f && polygonCollider != null && polygonCollider.WorldPoints.Any();
            var adjustForPixels = this.Settings.SnapToPixels && this.Transform.Position != this._previousPosition && amount.X != 0f;
            var settings = this.Settings;
            var platformPixelOffset = this.Transform.ToPixelSnappedValue(settings).Position.X - this.Transform.Position.X;

            foreach (var attached in this.Attached) {
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

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.Transform)) {
            this.MoveAttached(this.Transform.Position - this._previousPosition);
            this._previousPosition = this.Transform.Position;
        }
    }
}