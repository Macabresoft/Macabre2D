namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// A dynamic body.
    /// </summary>
    public sealed class DynamicBodyComponent : SimpleBodyComponent, IDynamicPhysicsBody {

        /// <inheritdoc/>
        [DataMember(Name = "Kinematic")]
        public bool IsKinematic { get; set; }

        /// <inheritdoc/>
        public float Mass { get; set; } = 1f;

        /// <inheritdoc/>
        public Vector2 Velocity { get; set; }
    }
}