namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private Vector2 _distanceAllowedFromActor = Vector2.Zero;

    /// <summary>
    /// Gets or sets the distance allowed from the player.
    /// </summary>
    [DataMember]
    public Vector2 DistanceAllowedFromActor {
        get => this._distanceAllowedFromActor;
        set => this.Set(ref this._distanceAllowedFromActor, value);
    }

    /// <inheritdoc />
    protected override bool IsTransformRelativeToParent => false;

    /// <summary>
    /// Updates the position according to the specified actor state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void UpdatePosition(FrameTime frameTime) {
        if (this.DistanceAllowedFromActor == Vector2.Zero) {
            this.LocalPosition = this.Parent.Transform.Position;
        }
        else if (this.Parent is PlatformerActor actor) {
            var (parentX, parentY) = actor.Transform.Position;
            var x = this.LocalPosition.X;
            var horizontalDistance = parentX - x;
            /*if (Math.Abs(actor.CurrentState.Velocity.X) > actor.MaximumHorizontalVelocity) {
                if (actor.CurrentState.FacingDirection == HorizontalDirection.Left) {
                    x = parentX - this.DistanceAllowedFromActor.X + this.FocusOffset.X;
                }
                else {
                    x = parentX + this.DistanceAllowedFromActor.X + this.FocusOffset.X;
                }
            }
            else {
                if (horizontalDistance > this.DistanceAllowedFromActor.X) {
                    x = parentX - this.DistanceAllowedFromActor.X + this.FocusOffset.X;
                }
                else if (horizontalDistance < -this.DistanceAllowedFromActor.X) {
                    x = parentX + this.DistanceAllowedFromActor.X + this.FocusOffset.X;
                }
            }*/

            if (horizontalDistance > this.DistanceAllowedFromActor.X) {
                x = parentX - this.DistanceAllowedFromActor.X;
            }
            else if (horizontalDistance < -this.DistanceAllowedFromActor.X) {
                x = parentX + this.DistanceAllowedFromActor.X;
            }

            var y = this.LocalPosition.Y;
            var verticalDistance = parentY - y;

            if (verticalDistance > this.DistanceAllowedFromActor.Y) {
                y = parentY - this.DistanceAllowedFromActor.Y;
            }
            else if (verticalDistance < -this.DistanceAllowedFromActor.Y) {
                y = parentY + this.DistanceAllowedFromActor.Y;
            }

            this.LocalPosition = new Vector2(x, y);
        }
    }
}