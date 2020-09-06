namespace Macabresoft.MonoGame.Core {

    using System;

    /// <summary>
    /// A wrapper for <see cref="SpriteAnimation" /> that allows it to be queued.
    /// </summary>
    public sealed class QueueableSpriteAnimation {

        /// <summary>
        /// An empty <see cref="QueueableSpriteAnimation" /> with no actual animation.
        /// </summary>
        public static readonly QueueableSpriteAnimation Empty = new QueueableSpriteAnimation(SpriteAnimation.Empty, false);

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueableSpriteAnimation" /> class.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="shouldLoopIndefinitely">
        /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
        /// animation has been queued.
        /// </param>
        public QueueableSpriteAnimation(SpriteAnimation animation, bool shouldLoopIndefinitely) : this(animation, shouldLoopIndefinitely, 1) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueableSpriteAnimation" /> class.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="shouldLoopIndefinitely">
        /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
        /// animation has been queued.
        /// </param>
        /// <param name="numberOfLoops">The number of loops.</param>
        public QueueableSpriteAnimation(SpriteAnimation animation, bool shouldLoopIndefinitely, ushort numberOfLoops) {
            this.Animation = animation ?? throw new ArgumentNullException(nameof(animation));
            this.ShouldLoopIndefinitely = shouldLoopIndefinitely;
            this.RemainingLoops = numberOfLoops;
        }

        /// <summary>
        /// Gets the animation.
        /// </summary>
        /// <value>The animation.</value>
        public SpriteAnimation Animation { get; }

        /// <summary>
        /// Gets or sets the number of remaining loops.
        /// </summary>
        /// <value>The number of remaining loops.</value>
        public ushort RemainingLoops { get; set; }

        /// <summary>
        /// Gets a value indicating whether this should loop indefinitely when no other animation
        /// has been queued.
        /// </summary>
        /// <value>
        /// <c>true</c> if this should loop indefinitely when no other animation has been queued;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool ShouldLoopIndefinitely { get; }
    }
}