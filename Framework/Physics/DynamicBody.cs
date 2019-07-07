namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// A dynamic body.
    /// </summary>
    public sealed class DynamicBody : Body {

        /// <summary>
        /// Gets or sets a value indicating whether this instance is kinematic.
        /// </summary>
        /// <value><c>true</c> if this instance is kinematic; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool IsKinematic { get; set; }

        /// <summary>
        /// Gets or sets the mass.
        /// </summary>
        /// <value>The mass.</value>
        public float Mass { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the velocity. This is always axis aligned.
        /// </summary>
        /// <value>The velocity.</value>
        public Vector2 Velocity { get; set; }
    }
}