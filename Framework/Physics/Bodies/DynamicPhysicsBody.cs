namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents a dynamic physics body, which can be moved and start collisions with other
/// bodies. It handles interactions with <see cref="Collider" />.
/// </summary>
public interface IDynamicPhysicsBody : IPhysicsBody {
    /// <summary>
    /// Gets or sets a value indicating whether this instance is kinematic.
    /// </summary>
    /// <value><c>true</c> if this instance is kinematic; otherwise, <c>false</c>.</value>
    bool IsKinematic { get; set; }

    /// <summary>
    /// Gets or sets the mass.
    /// </summary>
    /// <value>The mass.</value>
    float Mass { get; set; }

    /// <summary>
    /// Gets or sets the velocity. This is always axis aligned.
    /// </summary>
    /// <value>The velocity.</value>
    Vector2 Velocity { get; set; }
}

/// <summary>
/// A dynamic body.
/// </summary>
[Display(Name = "Dynamic Physics Body")]
public sealed class DynamicPhysicsBody : SimplePhysicsBody, IDynamicPhysicsBody {
    private bool _isKinematic;
    private float _mass = 1f;
    private Vector2 _velocity;

    /// <inheritdoc />
    [DataMember(Name = "Kinematic")]
    public bool IsKinematic {
        get => this._isKinematic;

        set => this.Set(ref this._isKinematic, value);
    }

    /// <inheritdoc />
    [DataMember]
    public float Mass {
        get => this._mass;

        set => this.Set(ref this._mass, value);
    }

    /// <inheritdoc />
    [DataMember]
    public Vector2 Velocity {
        get => this._velocity;

        set => this.Set(ref this._velocity, value);
    }
}