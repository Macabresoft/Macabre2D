namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// A sprite animator that can have animations queued up.
/// </summary>
[Display(Name = "Sprite Animator")]
[Category(CommonCategories.Animation)]
public class QueueableSpriteAnimator : BaseSpriteAnimator {
    private readonly Queue<QueueableSpriteAnimation> _queuedSpriteAnimations = new();

    /// <summary>
    /// An event for when an animation finishes.
    /// </summary>
    public event EventHandler<SpriteAnimation?>? OnAnimationFinished;

    /// <summary>
    /// Gets the currently queued animation if it exists.
    /// </summary>
    public QueueableSpriteAnimation? QueuedAnimation { get; private set; }

    /// <summary>
    /// Enqueues the specified animation.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="shouldLoopIndefinitely">
    /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
    /// animation has been queued.
    /// </param>
    public void Enqueue(SpriteAnimation animation, bool shouldLoopIndefinitely) {
        if (animation.SpriteSheet != null) {
            this.Enqueue(new QueueableSpriteAnimation(animation, shouldLoopIndefinitely));
        }
    }

    /// <summary>
    /// Enqueues the specified animation.
    /// </summary>
    /// <param name="animationReference">The animation reference.</param>
    /// <param name="shouldLoopIndefinitely">
    /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
    /// animation has been queued.
    /// </param>
    public void Enqueue(SpriteAnimationReference animationReference, bool shouldLoopIndefinitely) {
        if (animationReference.PackagedAsset is { } animation) {
            this.Enqueue(animation, shouldLoopIndefinitely);
        }
    }

    /// <summary>
    /// Plays the specified animation, which clears out the current queue and replaces the
    /// previous animation. If the animation is a looping animation, it will continue to play
    /// until a new animation is queued. If the animation is not a looping animation, it will
    /// pause on the final frame.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="shouldLoop">A value indicating whether or not the animation should loop.</param>
    public void Play(SpriteAnimation animation, bool shouldLoop) {
        if (this.QueuedAnimation != null) {
            this.OnAnimationFinished.SafeInvoke(this, this.QueuedAnimation.Animation);
        }

        this.QueuedAnimation = new QueueableSpriteAnimation(animation, shouldLoop);
        this._queuedSpriteAnimations.Clear();
        this.IsEnabled = true;
        this.IsPlaying = true;
    }

    /// <summary>
    /// Plays the specified animation, which clears out the current queue and replaces the
    /// previous animation. If the animation is a looping animation, it will continue to play
    /// until a new animation is queued. If the animation is not a looping animation, it will
    /// pause on the final frame.
    /// </summary>
    /// <param name="animationReference">The animation reference.</param>
    /// <param name="shouldLoop">A value indicating whether or not the animation should loop.</param>
    public void Play(SpriteAnimationReference animationReference, bool shouldLoop) {
        if (animationReference.PackagedAsset is { } animation) {
            this.Play(animation, shouldLoop);
        }
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public override void Stop() {
        this.Stop(true);
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    /// <param name="eraseQueue">A value indicating whether or not the queue should be erased.</param>
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
        else if (this.QueuedAnimation?.ShouldLoopIndefinitely == false) {
            var finishedAnimation = this.QueuedAnimation?.Animation;
            this.QueuedAnimation = null;
            this.OnAnimationFinished.SafeInvoke(this, finishedAnimation);
        }
    }

    private void Enqueue(QueueableSpriteAnimation queueableSpriteAnimation) {
        this._queuedSpriteAnimations.Enqueue(queueableSpriteAnimation);
    }
}