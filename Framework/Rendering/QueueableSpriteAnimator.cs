namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

/// <summary>
/// A sprite animator that can have animations queued up.
/// </summary>
[Display(Name = "Sprite Animator")]
[Category(CommonCategories.Animation)]
public sealed class QueueableSpriteAnimator : BaseSpriteAnimator {
    private readonly Queue<QueueableSpriteAnimation> _queuedSpriteAnimations = new();
    private QueueableSpriteAnimation? _currentAnimation;

    /// <inheritdoc />
    public override SpriteSheetAsset? SpriteSheet => this._currentAnimation?.Animation?.SpriteSheet;

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
    /// <param name="animation">The animation.</param>
    /// <param name="shouldLoopIndefinitely">
    /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
    /// animation has been queued.
    /// </param>
    /// <param name="numberOfLoops">The number of loops.</param>
    public void Enqueue(SpriteAnimation animation, bool shouldLoopIndefinitely, ushort numberOfLoops) {
        if (animation.SpriteSheet != null) {
            this.Enqueue(new QueueableSpriteAnimation(animation, shouldLoopIndefinitely, numberOfLoops));
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
        this.Stop(true);

        this._queuedSpriteAnimations.Clear();
        this.Enqueue(animation, shouldLoop);
        this.ResetAnimation();

        if (animation.Steps.FirstOrDefault() is SpriteAnimationStep step) {
            this.CurrentSpriteIndex = step.SpriteIndex;
        }

        this.Play();
    }

    /// <inheritdoc />
    public override void Stop() {
        this.Stop(true);
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    /// <param name="eraseQueue">A value indicating whether or not the queue should be erased.</param>
    public void Stop(bool eraseQueue) {
        base.Stop();

        if (eraseQueue) {
            this._currentAnimation = null;
            this._queuedSpriteAnimations.Clear();
        }
    }

    /// <inheritdoc />
    protected override SpriteAnimation? GetSpriteAnimation() {
        if (this._currentAnimation?.Animation == null && this._queuedSpriteAnimations.Any()) {
            this._currentAnimation = this._queuedSpriteAnimations.Dequeue();
        }

        return this._currentAnimation?.Animation;
    }

    /// <inheritdoc />
    protected override SpriteAnimation? HandleAnimationFinished() {
        if (this._queuedSpriteAnimations.Any()) {
            this._currentAnimation = this._queuedSpriteAnimations.Dequeue();
            var step = this._currentAnimation.Animation.Steps.FirstOrDefault();
            if (step != null) {
                this.CurrentSpriteIndex = step.SpriteIndex;
            }
            else {
                this._currentAnimation = null;
            }

            this.ResetMillisecondsPassed();
        }
        else if (this._currentAnimation?.ShouldLoopIndefinitely == false) {
            this._currentAnimation = null;
        }

        return this._currentAnimation?.Animation;
    }

    /// <summary>
    /// Enqueues the specified queueable sprite animation.
    /// </summary>
    /// <param name="queueableSpriteAnimation">The queueable sprite animation.</param>
    private void Enqueue(QueueableSpriteAnimation queueableSpriteAnimation) {
        this._queuedSpriteAnimations.Enqueue(queueableSpriteAnimation);
    }
}