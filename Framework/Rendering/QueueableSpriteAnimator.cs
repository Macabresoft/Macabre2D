namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A sprite animator that can have animations queued up.
/// </summary>
[Display(Name = "Sprite Animator")]
[Category(CommonCategories.Animation)]
public sealed class QueueableSpriteAnimator : BaseSpriteEntity, IUpdateableEntity {
    private readonly Queue<QueueableSpriteAnimation> _queuedSpriteAnimations = new();
    private QueueableSpriteAnimation? _currentAnimation;

    private byte _frameRate = 30;
    private bool _isPlaying;
    private int _millisecondsPerFrame;
    private int _updateOrder;

    /// <summary>
    /// An event for when an animation finishes.
    /// </summary>
    public event EventHandler<SpriteAnimation?>? OnAnimationFinished;

    /// <summary>
    /// Gets the current animation playing.
    /// </summary>
    public SpriteAnimation? CurrentAnimation => this._currentAnimation?.Animation;

    /// <summary>
    /// Gets a value indicating whether or not this is looping on the current animation.
    /// </summary>
    public bool IsLooping => this._currentAnimation is { ShouldLoopIndefinitely: true };

    public override byte? SpriteIndex => this._currentAnimation?.CurrentSpriteIndex;

    /// <summary>
    /// Gets or sets the frame rate. This is represented in frames per second.
    /// </summary>
    /// <value>The frame rate.</value>
    [DataMember(Order = 11, Name = "Frame Rate")]
    public byte FrameRate {
        get => this._frameRate;
        set {
            if (value != this._frameRate) {
                this._frameRate = value;
                this.ResetFrameRate();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not this is playing.
    /// </summary>
    public bool IsPlaying {
        get => this._isPlaying;
        private set => this.Set(ref this._isPlaying, value);
    }

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get => this._updateOrder;
        set => this.Set(ref this._updateOrder, value);
    }

    /// <inheritdoc />
    protected override SpriteSheetAsset? SpriteSheet => this._currentAnimation?.Animation.SpriteSheet;

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
    /// Gets the percentage complete for the current animation.
    /// </summary>
    /// <returns>The percentage complete.</returns>
    public float GetPercentageComplete() {
        var result = 0f;

        if (this._currentAnimation is { } animation) {
            result = animation.GetPercentageComplete();
        }

        return result;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.ResetFrameRate();
    }

    /// <summary>
    /// Pauses this instance.
    /// </summary>
    public void Pause() {
        this.IsPlaying = false;
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
        if (this._currentAnimation != null) {
            this.OnAnimationFinished.SafeInvoke(this, this._currentAnimation.Animation);
        }
        
        this._currentAnimation = new QueueableSpriteAnimation(animation, shouldLoop);
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
    public void Stop() {
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
            if (this._currentAnimation != null) {
                this.OnAnimationFinished.SafeInvoke(this, this._currentAnimation.Animation);
            }
            
            this._currentAnimation = null;
            this._queuedSpriteAnimations.Clear();
        }
        else {
            this._currentAnimation?.Reset();
        }
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        if (this.IsPlaying && this.GetCurrentAnimation() is { } animation) {
            animation.Update(frameTime, this._millisecondsPerFrame, out var isAnimationOver);

            if (isAnimationOver) {
                this.HandleAnimationFinished();
            }
        }
    }

    private void Enqueue(QueueableSpriteAnimation queueableSpriteAnimation) {
        this._queuedSpriteAnimations.Enqueue(queueableSpriteAnimation);
    }

    private QueueableSpriteAnimation? GetCurrentAnimation() {
        if (this._currentAnimation == null && this._queuedSpriteAnimations.Any()) {
            this._currentAnimation = this._queuedSpriteAnimations.Dequeue();
        }

        return this._currentAnimation;
    }

    private void HandleAnimationFinished() {
        if (this._queuedSpriteAnimations.Any()) {
            this.OnAnimationFinished.SafeInvoke(this, this._currentAnimation?.Animation);
            var millisecondsPassed = this._currentAnimation?.MillisecondsPassed ?? 0d;
            this._currentAnimation = this._queuedSpriteAnimations.Dequeue();
            this._currentAnimation.Reset();
            this._currentAnimation.MillisecondsPassed = millisecondsPassed;
        }
        else if (this._currentAnimation?.ShouldLoopIndefinitely == false) {
            this.OnAnimationFinished.SafeInvoke(this, this._currentAnimation?.Animation);
            this._currentAnimation = null;
        }
    }

    private void ResetFrameRate() {
        this._millisecondsPerFrame = 1000 / this._frameRate;
    }
}