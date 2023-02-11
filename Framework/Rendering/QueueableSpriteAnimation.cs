namespace Macabresoft.Macabre2D.Framework;

using System;

/// <summary>
/// A wrapper for <see cref="SpriteAnimation" /> that allows it to be queued.
/// </summary>
public sealed class QueueableSpriteAnimation {
    /// <summary>
    /// Initializes a new instance of the <see cref="QueueableSpriteAnimation" /> class.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="shouldLoopIndefinitely">
    /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
    /// animation has been queued.
    /// </param>
    public QueueableSpriteAnimation(SpriteAnimation animation, bool shouldLoopIndefinitely) {
        this.Animation = animation ?? throw new ArgumentNullException(nameof(animation));
        this.ShouldLoopIndefinitely = shouldLoopIndefinitely;
    }

    /// <summary>
    /// Gets the animation.
    /// </summary>
    /// <value>The animation.</value>
    public SpriteAnimation Animation { get; }

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