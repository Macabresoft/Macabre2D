namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// A material which describes the physical attributes of a physics body.
/// </summary>
[DataContract]
public sealed class PhysicsMaterial : IIdentifiable, INameable {
    /// <summary>
    /// The default physics material.
    /// </summary>
    public static readonly PhysicsMaterial Default = new(0.5f, 1f);

    /// <summary>
    /// An empty physics material with zero for both values.
    /// </summary>
    public static readonly PhysicsMaterial Empty = new(0f, 0f);

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicsMaterial" /> struct.
    /// </summary>
    /// <param name="bounce">The bounce.</param>
    /// <param name="friction">The friction.</param>
    /// <exception cref="System.NotSupportedException">
    /// Value for bounce must be greater than 0.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// Value for friction must be greater than 0.
    /// </exception>
    public PhysicsMaterial(float bounce, float friction) {
        if (bounce < 0f) {
            throw new NotSupportedException($"Value for {nameof(bounce)} must be greater than or equal to 0.");
        }

        if (friction < 0f) {
            throw new NotSupportedException($"Value for {nameof(friction)} must be greater than or equal to 0.");
        }

        this.Bounce = bounce;
        this.Friction = friction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicsMaterial" /> struct.
    /// </summary>
    public PhysicsMaterial() : this(Default.Bounce, Default.Friction) {
    }

    /// <summary>
    /// Gets or sets a multiplier used when another collider hits the collider with this physics material. A
    /// value of 0 means this object has no bounce.
    /// </summary>
    [DataMember]
    public float Bounce { get; set; }

    /// <summary>
    /// Gets or sets a multiplier used when two colliders are touching one another for an extended period.
    /// Friction will slow down a moving object.
    /// </summary>
    [DataMember]
    public float Friction { get; set; }

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; } = "New Physics Material";

    /// <inheritdoc />
    [DataMember]
    public Guid Id { get; set; } = Guid.NewGuid();
}