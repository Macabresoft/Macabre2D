namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents a rotation in degrees and radians.
/// </summary>
[DataContract]
public readonly struct Rotation {
    /// <summary>
    /// A rotation of zero degrees/radians.
    /// </summary>
    public static readonly Rotation Zero = new();

    /// <summary>
    /// The rotation represented in radians.
    /// </summary>
    [DataMember]
    public readonly float Radians;

    /// <summary>
    /// The rotation represented in degrees.
    /// </summary>
    [DataMember]
    public readonly float Degrees;

    /// <summary>
    /// Initializes a new instance of the <see cref="Rotation" /> struct.
    /// </summary>
    public Rotation() {
        this.Radians = 0f;
        this.Degrees = 0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Transform" /> struct.
    /// </summary>
    /// <param name="radians">The matrix.</param>
    public Rotation(float radians) {
        this.Radians = radians.NormalizeAngle();
        this.Degrees = MathHelper.ToDegrees(this.Radians);
    }

    /// <summary>
    /// Creates a rotation from degrees.
    /// </summary>
    /// <param name="degrees">The degrees.</param>
    /// <returns>The rotation.</returns>
    public static Rotation CreateFromDegrees(float degrees) {
        return new Rotation(MathHelper.ToRadians(degrees));
    }

    /// <summary>
    /// Creates a rotation from radians.
    /// </summary>
    /// <param name="radians">The radians.</param>
    /// <returns>The rotation.</returns>
    public static Rotation CreateFromRadians(float radians) {
        return new Rotation(radians);
    }

    /// <inheritdoc cref="object" />
    public override bool Equals(object? obj) {
        return obj is Rotation other && this.Equals(other);
    }

    /// <inheritdoc cref="object" />
    public bool Equals(Rotation other) {
        return this.Radians.Equals(other.Radians);
    }

    /// <inheritdoc cref="object" />
    public override int GetHashCode() {
        return this.Radians.GetHashCode();
    }

    /// <summary>
    /// Inverts values in the specified <see cref="Rotation" />.
    /// </summary>
    /// <param name="value">Source <see cref="Rotation" /> on the right of the sub sign.</param>
    /// <returns>Result of the inversion.</returns>
    public static Rotation operator -(Rotation value) {
        return new Rotation(-value.Radians);
    }

    /// <summary>Adds two <see cref="Rotation" /> values.</summary>
    /// <param name="value1">Source <see cref="Rotation" /> on the left of the add sign.</param>
    /// <param name="value2">Source <see cref="Rotation" /> on the right of the add sign.</param>
    /// <returns>Sum of the values.</returns>
    public static Rotation operator +(Rotation value1, Rotation value2) {
        return new Rotation(value1.Radians + value2.Radians);
    }

    /// <summary>
    /// Subtracts a <see cref="Rotation" /> from a <see cref="Rotation" />.
    /// </summary>
    /// <param name="value1">Source <see cref="Rotation" /> on the left of the subtract sign.</param>
    /// <param name="value2">Source <see cref="Rotation" /> on the right of the subtract sign.</param>
    /// <returns>Result of the subtraction.</returns>
    public static Rotation operator -(Rotation value1, Rotation value2) {
        return new Rotation(value1.Radians - value2.Radians);
    }

    /// <summary>Adds two <see cref="Rotation" /> values.</summary>
    /// <param name="value1">Source <see cref="Rotation" /> on the left of the add sign.</param>
    /// <param name="value2">Source <see cref="float" /> on the right of the add sign.</param>
    /// <returns>Sum of the values..</returns>
    public static Rotation operator +(Rotation value1, float value2) {
        return new Rotation(value1.Radians + value2);
    }

    /// <summary>
    /// Subtracts a <see cref="Rotation" /> from a <see cref="Rotation" />.
    /// </summary>
    /// <param name="value1">Source <see cref="Rotation" /> on the left of the subtract sign.</param>
    /// <param name="value2">Source <see cref="float" /> on the right of the subtract sign.</param>
    /// <returns>Result of the subtraction.</returns>
    public static Rotation operator -(Rotation value1, float value2) {
        return new Rotation(value1.Radians - value2);
    }
}