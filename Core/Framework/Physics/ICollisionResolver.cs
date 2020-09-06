namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// Interface for resolving collisions.
    /// </summary>
    public interface ICollisionResolver {

        /// <summary>
        /// Initializes the collision resolver, providing it the physics service.
        /// </summary>
        /// <param name="service">The service.</param>
        void Initialize(IGamePhysicsService service);

        /// <summary>
        /// Resolves the collision.
        /// </summary>
        /// <param name="e">
        /// The <see cref="CollisionEventArgs" /> instance containing the event data.
        /// </param>
        /// <param name="timeStep">The time step.</param>
        void ResolveCollision(CollisionEventArgs e, float timeStep);
    }
}