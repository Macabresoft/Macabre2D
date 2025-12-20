namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// Extension methods for <see cref="IBoundableEntity" /> entities.
/// </summary>
public static class BoundableExtensions {

    /// <param name="boundable">The entity that implements <see cref="IBoundableEntity" /> and <see cref="ITransformable" />.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    extension<T>(T boundable) where T : IBoundableEntity, ITransformable {
        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the bottom boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetBottomBoundaryTo(float position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var worldPosition = boundable.WorldPosition;
                var difference = worldPosition.Y - boundable.BoundingArea.Minimum.Y;
                boundable.SetWorldPosition(new Vector2(worldPosition.X, difference + position));
            }
        }

        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the bottom left boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetBottomLeftBoundaryTo(Vector2 position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var difference = boundable.WorldPosition - boundable.BoundingArea.Minimum;
                boundable.SetWorldPosition(difference + position);
            }
        }

        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the bottom right boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetBottomRightBoundaryTo(Vector2 position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var worldPosition = boundable.WorldPosition;
                var difference = new Vector2(worldPosition.X - boundable.BoundingArea.Maximum.X, worldPosition.Y - boundable.BoundingArea.Minimum.Y);
                boundable.SetWorldPosition(difference + position);
            }
        }

        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the left boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetLeftBoundaryTo(float position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var worldPosition = boundable.WorldPosition;
                var difference = worldPosition.X - boundable.BoundingArea.Minimum.X;
                boundable.SetWorldPosition(new Vector2(difference + position, worldPosition.Y));
            }
        }

        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the right boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetRightBoundaryTo(float position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var worldPosition = boundable.WorldPosition;
                var difference = worldPosition.X - boundable.BoundingArea.Maximum.X;
                boundable.SetWorldPosition(new Vector2(difference + position, worldPosition.Y));
            }
        }

        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the top boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetTopBoundaryTo(float position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var worldPosition = boundable.WorldPosition;
                var difference = worldPosition.Y - boundable.BoundingArea.Maximum.Y;
                boundable.SetWorldPosition(new Vector2(worldPosition.X, difference + position));
            }
        }

        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the top left boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetTopBoundBoundaryTo(Vector2 position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var worldPosition = boundable.WorldPosition;
                var difference = new Vector2(worldPosition.X - boundable.BoundingArea.Minimum.X, worldPosition.Y - boundable.BoundingArea.Maximum.Y);
                boundable.SetWorldPosition(difference + position);
            }
        }

        /// <summary>
        /// Sets the world position of the <see cref="IBoundableEntity" /> so that the top right boundary is in the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetTopRightBoundaryTo(Vector2 position) {
            if (!boundable.BoundingArea.IsEmpty) {
                var difference = boundable.WorldPosition - boundable.BoundingArea.Maximum;
                boundable.SetWorldPosition(difference + position);
            }
        }
    }
}