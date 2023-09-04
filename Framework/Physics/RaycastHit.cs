namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// Represents a hit between a ray and a collider.
/// </summary>
public sealed class RaycastHit {
    /// <summary>
    /// The empty raycast hit.
    /// </summary>
    public static readonly RaycastHit Empty = new(Collider.Empty, Vector2.Zero, Vector2.Zero);

    /// <summary>
    /// Initializes a new instance of the <see cref="RaycastHit" /> class.
    /// </summary>
    /// <param name="collider">The collider.</param>
    /// <param name="contactPoint">The contact point.</param>
    /// <param name="normal">The normal.</param>
    public RaycastHit(Collider collider, Vector2 contactPoint, Vector2 normal) {
        this.Collider = collider;
        this.ContactPoint = contactPoint;
        this.Normal = normal;
    }

    /// <summary>
    /// Gets the collider.
    /// </summary>
    /// <value>The collider.</value>
    public Collider Collider { get; }

    /// <summary>
    /// Gets the contact point.
    /// </summary>
    /// <value>The contact point.</value>
    public Vector2 ContactPoint { get; }

    /// <summary>
    /// Gets the normal of the hit.
    /// </summary>
    /// <value>The normal.</value>
    public Vector2 Normal { get; }
}