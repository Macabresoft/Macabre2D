namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// Represents a physics body that handles interactions with <see cref="Collider" />.
    /// </summary>
    public interface IPhysicsBody : IEntity, IBoundable {
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

    /// <summary>
    /// A base for entities that implement <see cref="IPhysicsBody" />.
    /// </summary>
    [Display(Name = "Physics Body")]
    [Category("Body")]
    public abstract class PhysicsBody : Entity, IPhysicsBody {
        private int _updateOrder;

        /// <inheritdoc />
        public event EventHandler<CollisionEventArgs>? CollisionOccured;

        /// <inheritdoc />
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc />
        public abstract bool HasCollider { get; }

        /// <inheritdoc />
        [DataMember(Order = 1, Name = "Use as Trigger")]
        public bool IsTrigger { get; set; }

        /// <inheritdoc />
        [DataMember(Order = 2, Name = "Physics Material")]
        public PhysicsMaterial PhysicsMaterial { get; set; } = PhysicsMaterial.Default;

        /// <inheritdoc />
        [DataMember]
        public int UpdateOrder {
            get => this._updateOrder;
            set => this.Set(ref this._updateOrder, value);
        }

        /// <inheritdoc />
        public abstract IEnumerable<Collider> GetColliders();

        /// <inheritdoc />
        public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
            this.CollisionOccured.SafeInvoke(this, eventArgs);
        }
    }
}