namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Extensions for <see cref="IMovingPlatform"/>.
/// </summary>
public static class MovingPlatformExtensions {
    /// <summary>
    /// Moves the attached actors on a moving platform.
    /// </summary>
    /// <param name="platform">The platform.</param>
    public static void MoveAttached(this IMovingPlatform platform) {
        if (platform.Attached.Any()) {
            // TODO: this can only hand polygon colliders that are flat. Expand for any collider and find the actual collision spot.
            var attachedMovement = platform.Transform.Position - platform.PreviousPosition;
            var polygonCollider = platform.Collider as PolygonCollider;
            var adjustForY = platform.MovesVertically && polygonCollider != null && polygonCollider.WorldPoints.Any();
            var settings = platform.Scene.Game.Project.Settings;
            var platformPixelOffset = platform.Transform.ToPixelSnappedValue(settings).Position.X - platform.Transform.Position.X;

            foreach (var attached in platform.Attached) {
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