namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A material which describes the physical attributes of a physics body.
/// </summary>
[DataContract]
public sealed class PhysicsMaterial : PropertyChangedNotifier, IIdentifiable, INameable {
    /// <summary>
    /// The default physics material.
    /// </summary>
    public static readonly PhysicsMaterial Default = new(0.5f, 1f) {
        Id = new Guid("cb78d13e-b7d4-4877-bb8b-bdf910f0e839"),
        Name = "Default"
    };

    /// <summary>
    /// An empty physics material with zero for both values.
    /// </summary>
    public static readonly PhysicsMaterial Empty = new(0f, 0f) {
        Id = Guid.Empty,
        Name = "Empty"
    };

    private string _name = "New Physics Material";

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
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public string Name {
        get => this._name;
        set => this.Set(ref this._name, value);
    }
}