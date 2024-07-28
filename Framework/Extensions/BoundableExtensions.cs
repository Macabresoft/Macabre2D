namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// Extension methods for <see cref="IBoundable" /> entities.
/// </summary>
public static class BoundableExtensions {

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the bottom boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetBottomBoundaryTo<T>(this T boundable, float position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var worldPosition = boundable.WorldPosition;
            var difference = worldPosition.Y - boundable.BoundingArea.Minimum.Y;
            boundable.SetWorldPosition(new Vector2(worldPosition.X, difference + position));
        }
    }

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the bottom left boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetBottomLeftBoundaryTo<T>(this T boundable, Vector2 position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var difference = boundable.WorldPosition - boundable.BoundingArea.Minimum;
            boundable.SetWorldPosition(difference + position);
        }
    }

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the bottom right boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetBottomRightBoundaryTo<T>(this T boundable, Vector2 position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var worldPosition = boundable.WorldPosition;
            var difference = new Vector2(worldPosition.X - boundable.BoundingArea.Maximum.X, worldPosition.Y - boundable.BoundingArea.Minimum.Y);
            boundable.SetWorldPosition(difference + position);
        }
    }

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the left boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetLeftBoundaryTo<T>(this T boundable, float position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var worldPosition = boundable.WorldPosition;
            var difference = worldPosition.X - boundable.BoundingArea.Minimum.X;
            boundable.SetWorldPosition(new Vector2(difference + position, worldPosition.Y));
        }
    }

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the right boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetRightBoundaryTo<T>(this T boundable, float position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var worldPosition = boundable.WorldPosition;
            var difference = worldPosition.X - boundable.BoundingArea.Maximum.X;
            boundable.SetWorldPosition(new Vector2(difference + position, worldPosition.Y));
        }
    }

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the top boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetTopBoundaryTo<T>(this T boundable, float position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var worldPosition = boundable.WorldPosition;
            var difference = worldPosition.Y - boundable.BoundingArea.Maximum.Y;
            boundable.SetWorldPosition(new Vector2(worldPosition.X, difference + position));
        }
    }

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the top left boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetTopBoundBoundaryTo<T>(this T boundable, Vector2 position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var worldPosition = boundable.WorldPosition;
            var difference = new Vector2(worldPosition.X - boundable.BoundingArea.Minimum.X, worldPosition.Y - boundable.BoundingArea.Maximum.Y);
            boundable.SetWorldPosition(difference + position);
        }
    }

    /// <summary>
    /// Sets the world position of the <see cref="IBoundable" /> so that the top right boundary is in the specified position.
    /// </summary>
    /// <param name="boundable">The entity that implements <see cref="IBoundable" /> and <see cref="ITransformable" />.</param>
    /// <param name="position">The position.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public static void SetTopRightBoundaryTo<T>(this T boundable, Vector2 position) where T : IBoundable, ITransformable {
        if (!boundable.BoundingArea.IsEmpty) {
            var difference = boundable.WorldPosition - boundable.BoundingArea.Maximum;
            boundable.SetWorldPosition(difference + position);
        }
    }
}