namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// Interface for a <see cref="ISpriteAnimator" /> which can queue up animations.
/// </summary>
public interface IQueueableSpriteAnimator : ISpriteAnimator {
    /// <summary>
    /// An event for when an animation finishes.
    /// </summary>
    event EventHandler<SpriteAnimation?>? OnAnimationFinished;

    /// <summary>
    /// Gets the currently queued animation if it exists.
    /// </summary>
    QueueableSpriteAnimation? QueuedAnimation { get; }

    /// <summary>
    /// Enqueues the specified animation.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="shouldLoop">
    /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
    /// animation has been queued.
    /// </param>
    void Enqueue(SpriteAnimation animation, bool shouldLoop);

    /// <summary>
    /// Enqueues the specified animation.
    /// </summary>
    /// <param name="animationReference">The animation reference.</param>
    /// <param name="shouldLoop">
    /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
    /// animation has been queued.
    /// </param>
    void Enqueue(SpriteAnimationReference animationReference, bool shouldLoop);

    /// <summary>
    /// Plays the specified animation, which clears out the current queue and replaces the
    /// previous animation. If the animation is a looping animation, it will continue to play
    /// until a new animation is queued. If the animation is not a looping animation, it will
    /// pause on the final frame.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="shouldLoop">A value indicating whether the animation should loop.</param>
    void Play(SpriteAnimation animation, bool shouldLoop);

    /// <summary>
    /// Plays the specified animation, which clears out the current queue and replaces the
    /// previous animation. If the animation is a looping animation, it will continue to play
    /// until a new animation is queued. If the animation is not a looping animation, it will
    /// pause on the final frame.
    /// </summary>
    /// <param name="animationReference">The animation reference.</param>
    /// <param name="shouldLoop">A value indicating whether the animation should loop.</param>
    void Play(SpriteAnimationReference animationReference, bool shouldLoop);

    /// <summary>
    /// Sets the percentage complete of the current animation to a value between 0 and 1.
    /// </summary>
    /// <param name="amount">The amount.</param>
    void SetPercentageComplete(float amount);

    /// <summary>
    /// Stops this instance.
    /// </summary>
    /// <param name="eraseQueue">A value indicating whether the queue should be erased.</param>
    void Stop(bool eraseQueue);

    /// <summary>
    /// Swaps in one animation for another while maintaining everything about the current frame and transposing it onto the new animation.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <remarks>
    /// This will only work if the new animation has the same or more frames than the current animation.
    /// </remarks>
    void Swap(SpriteAnimation animation);

    /// <summary>
    /// Swaps in one animation for another while maintaining everything about the current frame and transposing it onto the new animation.
    /// </summary>
    /// <remarks>
    /// This will only work if the new animation has the same or more frames than the current animation.
    /// </remarks>
    void Swap(SpriteAnimationReference animationReference);
}

/// <summary>
/// A sprite animator that can have animations queued up.
/// </summary>
[Category(CommonCategories.Animation)]
public class QueueableSpriteAnimator : BaseSpriteAnimator, IQueueableSpriteAnimator {
    private readonly Queue<QueueableSpriteAnimation> _queuedSpriteAnimations = new();

    /// <inheritdoc />
    public event EventHandler<SpriteAnimation?>? OnAnimationFinished;

    /// <inheritdoc />
    public QueueableSpriteAnimation? QueuedAnimation { get; private set; }

    /// <inheritdoc />
    public void Enqueue(SpriteAnimation animation, bool shouldLoop) {
        if (animation.SpriteSheet != null) {
            this.Enqueue(new QueueableSpriteAnimation(animation, shouldLoop ? AnimationLoopKind.Repeating : AnimationLoopKind.None));
        }
    }

    /// <inheritdoc />
    public void Enqueue(SpriteAnimationReference animationReference, bool shouldLoop) {
        if (animationReference.PackagedAsset is { } animation) {
            this.Enqueue(animation, shouldLoop);
        }
    }

    /// <inheritdoc />
    public void Play(SpriteAnimation animation, bool shouldLoop) {
        if (this.QueuedAnimation != null) {
            this.OnAnimationFinished.SafeInvoke(this, this.QueuedAnimation.Animation);
        }

        this.QueuedAnimation = new QueueableSpriteAnimation(animation, shouldLoop ? AnimationLoopKind.Repeating : AnimationLoopKind.None);
        this._queuedSpriteAnimations.Clear();
        this.IsEnabled = true;
        this.IsPlaying = true;
    }

    /// <inheritdoc />
    public void Play(SpriteAnimationReference animationReference, bool shouldLoop) {
        if (animationReference.PackagedAsset is { } animation) {
            this.Play(animation, shouldLoop);
        }
    }

    /// <inheritdoc />
    public void SetPercentageComplete(float amount) {
        if (this.GetCurrentAnimation() is { } animation) {
            animation.SetPercentageComplete(amount);
        }
    }

    /// <inheritdoc />
    public override void Stop() {
        this.Stop(true);
    }

    /// <inheritdoc />
    public void Stop(bool eraseQueue) {
        this.IsEnabled = false;
        this.IsPlaying = false;

        if (eraseQueue) {
            if (this.QueuedAnimation != null) {
                this.OnAnimationFinished.SafeInvoke(this, this.QueuedAnimation.Animation);
            }

            this.QueuedAnimation = null;
            this._queuedSpriteAnimations.Clear();
        }
        else {
            this.QueuedAnimation?.Reset();
        }
    }

    /// <inheritdoc />
    public void Swap(SpriteAnimation animation) {
        this.GetCurrentAnimation()?.Swap(animation);
    }

    /// <inheritdoc />
    public void Swap(SpriteAnimationReference animationReference) {
        if (animationReference.PackagedAsset is { } animation) {
            this.Swap(animation);
        }
    }

    /// <inheritdoc />
    protected override QueueableSpriteAnimation? GetCurrentAnimation() {
        if (this.QueuedAnimation == null && this._queuedSpriteAnimations.Any()) {
            this.QueuedAnimation = this._queuedSpriteAnimations.Dequeue();
        }

        return this.QueuedAnimation;
    }

    /// <inheritdoc />
    protected override void HandleAnimationFinished() {
        if (this._queuedSpriteAnimations.Any()) {
            var finishedAnimation = this.QueuedAnimation?.Animation;
            var millisecondsPassed = this.QueuedAnimation?.MillisecondsPassed ?? 0d;
            this.QueuedAnimation = this._queuedSpriteAnimations.Dequeue();
            this.QueuedAnimation.Reset();
            this.QueuedAnimation.MillisecondsPassed = millisecondsPassed;
            this.OnAnimationFinished.SafeInvoke(this, finishedAnimation);
        }
        else if (this.QueuedAnimation?.LoopKind == AnimationLoopKind.None) {
            var finishedAnimation = this.QueuedAnimation?.Animation;
            this.QueuedAnimation = null;
            this.OnAnimationFinished.SafeInvoke(this, finishedAnimation);
        }
    }

    private void Enqueue(QueueableSpriteAnimation queueableSpriteAnimation) {
        this._queuedSpriteAnimations.Enqueue(queueableSpriteAnimation);
    }
}