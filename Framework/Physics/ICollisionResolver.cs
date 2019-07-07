namespace Macabre2D.Framework {

    /// <summary>
    /// Interface for resolving collisions.
    /// </summary>
    public interface ICollisionResolver {

        /// <summary>
        /// Initializes the collision resolver, providing it the physics module.
        /// </summary>
        /// <param name="module">The module.</param>
        void Initialize(ICollisionBasedPhysicsModule module);

        /// <summary>
        /// Resolves the collision.
        /// </summary>
        /// <param name="e">
        /// The <see cref="CollisionEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="timeStep">The time step.</param>
        void ResolveCollision(CollisionEventArgs e, float timeStep);
    }
}