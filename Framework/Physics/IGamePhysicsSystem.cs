namespace Macabresoft.Macabre2D.Framework {
    /// <summary>
    /// A physics system that handles collisions.
    /// </summary>
    public interface IGamePhysicsSystem {
        /// <summary>
        /// Gets or sets the gravity.
        /// </summary>
        /// <value>The gravity.</value>
        Gravity Gravity { get; }

        /// <summary>
        /// Gets the groundedness. This is a value indicating how likely a body is to be grounded.
        /// It should be a value between 0 and 1, where 0.5 would (theoretically) allow an object to
        /// be grounded at a 45 degree angle.
        /// </summary>
        /// <value>The groundedness.</value>
        float Groundedness { get; }

        /// <summary>
        /// Gets the minimum magnitude of velocity perpendicular to the collision after bounce has
        /// been applied.
        /// </summary>
        /// <value>The minimum post bounce magnitude.</value>
        float MinimumPostBounceMagnitude { get; }

        /// <summary>
        /// Gets the minimum magnitude of velocity parallel to the collision after friction has been applied.
        /// </summary>
        /// <value>The minimum post friction magnitude.</value>
        float MinimumPostFrictionMagnitude { get; }

        /// <summary>
        /// Gets the stickiness. This is a value indicating how likely a body is to "stick" to
        /// another object when it is moving in a similar path to the other body's edge. This should
        /// be a value between 0 and 1, with 0 not being sticky at all and 1 sticking all of the time.
        /// </summary>
        /// <value>The stickiness.</value>
        float Stickiness { get; }
    }
}