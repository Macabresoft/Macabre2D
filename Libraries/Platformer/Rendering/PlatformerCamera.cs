namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private float _horizontalDistanceFromActorAllowed = 2f;

    /// <summary>
    /// Gets or sets the horizontal distance allowed from the player.
    /// </summary>
    [DataMember]
    public float HorizontalDistanceFromActorAllowed {
        get => this._horizontalDistanceFromActorAllowed;
        set => this.Set(ref this._horizontalDistanceFromActorAllowed, value);
    }

    /// <inheritdoc />
    protected override bool IsTransformRelativeToParent => false;

    /// <summary>
    /// Updates the position according to the specified actor state.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void UpdatePosition(FrameTime frameTime) {
        if (this.HorizontalDistanceFromActorAllowed == 0f) {
            this.LocalPosition = this.Parent.Transform.Position;
        }
        else if (this.Parent is PlatformerPlayer actor) {
            var (parentX, parentY) = actor.Transform.Position;
            var x = this.LocalPosition.X;
            var horizontalDistance = parentX - x;
            /*if (actor.MaximumHorizontalVelocity >= actor.RunVelocity) {
                if (actor.CurrentState.FacingDirection == HorizontalDirection.Left) {
                    x = parentX - this.HorizontalDistanceFromActorAllowed;
                }
                else {
                    x = parentX + this.HorizontalDistanceFromActorAllowed;
                }
            }
            else {
                if (horizontalDistance > this.HorizontalDistanceFromActorAllowed) {
                    x = parentX - this.HorizontalDistanceFromActorAllowed;
                }
                else if (horizontalDistance < -this.HorizontalDistanceFromActorAllowed) {
                    x = parentX + this.HorizontalDistanceFromActorAllowed;
                }
            }*/

            if (horizontalDistance > this.HorizontalDistanceFromActorAllowed) {
                x = parentX - this.HorizontalDistanceFromActorAllowed;
            }
            else if (horizontalDistance < -this.HorizontalDistanceFromActorAllowed) {
                x = parentX + this.HorizontalDistanceFromActorAllowed;
            }

            this.LocalPosition = new Vector2(x, parentY);
        }
    }
}