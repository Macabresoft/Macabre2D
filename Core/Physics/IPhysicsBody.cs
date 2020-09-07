namespace Macabresoft.MonoGame.Core {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a physics body that handles interactions with <see cref="Collider" />.
    /// </summary>
    public interface IPhysicsBody : IGameComponent, IBoundable {

        /// <summary>
        /// Occurs when a collision occurs involving this body.
        /// </summary>
        event EventHandler<CollisionEventArgs> CollisionOccured;

        /// <summary>
        /// Gets a value indicating whether this instance has a collider.
        /// </summary>
        /// <value><c>true</c> if this instance has a collider; otherwise, <c>false</c>.</value>
        bool HasCollider { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is trigger. If it is a trigger, instead of
        /// reacting within the physics system, it will simply notify that a collision has occured
        /// and do nothing else.
        /// </summary>
        /// <value><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</value>
        bool IsTrigger { get; }

        /// <summary>
        /// Gets or sets the physics material.
        /// </summary>
        /// <value>The physics material.</value>
        PhysicsMaterial PhysicsMaterial { get; }

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }

        /// <summary>
        /// Gets the colliders attached to this body.
        /// </summary>
        /// <returns>The colliders.</returns>
        IEnumerable<Collider> GetColliders();

        /// <summary>
        /// Raises an event to notify that a collision occured on this body.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="CollisionEventArgs" /> instance containing the event data.
        /// </param>
        void NotifyCollisionOccured(CollisionEventArgs eventArgs);
    }
}