namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

/// <summary>
/// Extension methods for vectors.
/// </summary>
public static class VectorExtensions {
    /// <summary>
    /// Gets the average of an array of vectors.
    /// </summary>
    /// <param name="vectors">The vectors.</param>
    /// <returns>A vector that should be in the mathematical center of all specified vectors.</returns>
    public static Vector2 GetAverage(this IEnumerable<Vector2> vectors) {
        var count = vectors.Count();
        if (count == 0) {
            return Vector2.Zero;
        }

        var x = 0f;
        var y = 0f;

        foreach (var vector in vectors) {
            x += vector.X;
            y += vector.Y;
        }

        return new Vector2(x / count, y / count);
    }

    /// <summary>
    /// Gets the line end point.
    /// </summary>
    /// <param name="startPoint">The start point.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="length">The length.</param>
    /// <returns>The end point of a line.</returns>
    public static Vector2 GetLineEndPoint(this Vector2 startPoint, Vector2 direction, float length) {
        return startPoint + direction.GetNormalized() * length;
    }

    /// <summary>
    /// Gets the normalized version of the vector.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The normalized version of the vector.</returns>
    public static Vector2 GetNormalized(this Vector2 vector) {
        vector.Normalize();
        return vector;
    }

    /// <summary>
    /// Gets the vector perpindicular (clockwise) to the provided vector.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The vector perpindicular (clockwise) to the provided vector.</returns>
    public static Vector2 GetPerpendicular(this Vector2 vector) {
        return new Vector2(-vector.Y, vector.X);
    }

    /// <summary>
    /// Gets the vector perpindicular (counter clockwise) to the provided vector.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The vector perpindicular (counter clockwise) to the provided vector.</returns>
    public static Vector2 GetPerpendicularCounterClockwise(this Vector2 vector) {
        return new Vector2(vector.Y, -vector.X);
    }

    /// <summary>
    /// Determines whether two floating point values have minimum difference.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <param name="difference">The acceptable difference to be considered equal.</param>
    /// <returns><c>true</c> if there is minimal difference; otherwise, <c>false</c>.</returns>
    public static bool HasMinimalDifference(this Vector2 value1, Vector2 value2, float difference = 0.00001f) {
        var VectorDifference = value1 - value2;
        return Math.Abs(VectorDifference.X) <= difference && Math.Abs(VectorDifference.Y) <= difference;
    }

    /// <summary>
    /// Rotates a vector by the specified number of degrees.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>The rotated vector.</returns>
    public static Vector2 RotateDegrees(this Vector2 vector, float degrees) {
        return vector.RotateRadians(MathHelper.ToRadians(degrees));
    }

    /// <summary>
    /// Rotates a vector by the specified number of radians.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="radians">The angle in radians.</param>
    /// <returns>The rotated vector</returns>
    public static Vector2 RotateRadians(this Vector2 vector, float radians) {
        return new Vector2(
            (float)(vector.X * Math.Cos(radians) - vector.Y * Math.Sin(radians)),
            (float)(vector.X * Math.Sin(radians) - vector.Y * Math.Cos(radians)));
    }

    /// <summary>
    /// Translates a Vector3 into a Vector2 by cutting off the Z value.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>A Vector2 from the Vector3.</returns>
    public static Vector2 ToVector2(this Vector3 vector) {
        return new Vector2(vector.X, vector.Y);
    }

    /// <summary>
    /// Translates a Vector2 into a Vector3 by appending a Z value of 0.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>A Vector3 from the Vector2.</returns>
    public static Vector3 ToVector3(this Vector2 vector) {
        return new Vector3(vector, 0f);
    }

    /// <summary>
    /// Translates a Vector2 into a Vector3 by appending the specified z value.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="zValue">The z value.</param>
    /// <returns>A Vector3 from the Vector2.</returns>
    public static Vector3 ToVector3(this Vector2 vector, float zValue) {
        return new Vector3(vector, zValue);
    }
}