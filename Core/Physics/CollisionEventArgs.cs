namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// The information provided after a collision occurs.
    /// </summary>
    public sealed class CollisionEventArgs : EventArgs {

        /// <summary>
        /// Empty event arguments.
        /// </summary>
        public static readonly CollisionEventArgs Empty = new CollisionEventArgs(Collider.Empty, Collider.Empty, Vector2.Zero, Vector2.Zero, false, false);

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionEventArgs" /> class.
        /// </summary>
        /// <param name="firstCollider">The first collider.</param>
        /// <param name="secondCollider">The second collider.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="minimumTranslation">The minimum translation.</param>
        /// <param name="firstContainsSecond">if set to <c>true</c> [first contains second].</param>
        /// <param name="secondContainsFirst">if set to <c>true</c> [second contains first].</param>
        public CollisionEventArgs(Collider firstCollider, Collider secondCollider, Vector2 normal, Vector2 minimumTranslation, bool firstContainsSecond, bool secondContainsFirst) {
            this.FirstCollider = firstCollider;
            this.SecondCollider = secondCollider;
            this.Normal = normal;
            this.MinimumTranslationVector = minimumTranslation;
            this.FirstContainsSecond = firstContainsSecond;
            this.SecondContainsFirst = secondContainsFirst;
        }

        /// <summary>
        /// Gets the first collider.
        /// </summary>
        /// <value>The first collider.</value>
        public Collider FirstCollider { get; }

        /// <summary>
        /// Gets a value indicating whether the first collider contains the second collider.
        /// </summary>
        /// <value><c>true</c> if the first collider contains the second collider; otherwise, <c>false</c>.</value>
        public bool FirstContainsSecond { get; }

        /// <summary>
        /// Gets the minimum translation vector. When applied to the first collider, these two
        /// objects will no longer be colliding.
        /// </summary>
        /// <value>The minimum translation vector.</value>
        public Vector2 MinimumTranslationVector { get; }

        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector2 Normal { get; }

        /// <summary>
        /// Gets the second collider.
        /// </summary>
        /// <value>The second collider.</value>
        public Collider SecondCollider { get; }

        /// <summary>
        /// Gets a value indicating whether the second collider contains the first collider.
        /// </summary>
        /// <value><c>true</c> if the second collider contains the first collider; otherwise, <c>false</c>.</value>
        public bool SecondContainsFirst { get; }
    }
}