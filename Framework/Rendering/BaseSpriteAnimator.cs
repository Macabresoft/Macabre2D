﻿namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>
/// A base class for sprite animators.
/// </summary>
public abstract class BaseSpriteAnimator : BaseSpriteEntity, IAnimatableEntity {
    private bool _isPlaying;

    /// <inheritdoc />
    public SpriteAnimation? CurrentAnimation => this.GetCurrentAnimation()?.Animation;

    /// <inheritdoc />
    [DataMember(Order = 11, Name = "Frame Rate")]
    public ByteOverride FrameRateOverride { get; } = new();

    /// <inheritdoc />
    public bool IsLooping => this.GetCurrentAnimation() is { ShouldLoopIndefinitely: true };

    /// <inheritdoc />
    public override byte? SpriteIndex => this.GetCurrentAnimation()?.CurrentSpriteIndex;

    /// <inheritdoc />
    public bool IsPlaying {
        get => this._isPlaying;
        protected set => this.Set(ref this._isPlaying, value);
    }

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this.CurrentAnimation?.SpriteSheet;

    /// <summary>
    /// Get the number of milliseconds in a single frame.
    /// </summary>
    protected int MillisecondsPerFrame { get; private set; }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.FrameRateOverride.PropertyChanged -= this.FrameRateOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public float GetPercentageComplete() {
        var result = 0f;

        if (this.GetCurrentAnimation() is { } animation) {
            result = animation.GetPercentageComplete();
        }

        return result;
    }

    /// <inheritdoc />
    public virtual void IncrementTime(FrameTime frameTime) {
        if (this.TryGetCurrentAnimation(out var animation)) {
            animation.Update(frameTime, this.MillisecondsPerFrame, out var isAnimationOver);

            if (isAnimationOver) {
                this.HandleAnimationFinished();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.ResetFrameRate();
        this.FrameRateOverride.PropertyChanged += this.FrameRateOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public virtual void NextFrame() {
        if (this.TryGetCurrentAnimation(out var animation)) {
            animation.TryNextFrame(out var isAnimationOver);
            if (isAnimationOver) {
                this.HandleAnimationFinished();
            }
        }
    }

    /// <summary>
    /// Pauses this instance.
    /// </summary>
    public void Pause() {
        this.IsPlaying = false;
    }

    /// <summary>
    /// Plays this instance.
    /// </summary>
    public void Play() {
        this.IsEnabled = true;
        this.IsPlaying = true;
        this.Reset();
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public virtual void Stop() {
        this.IsEnabled = false;
        this.IsPlaying = false;
        this.ResetAnimation();
    }

    /// <summary>
    /// Gets the sprite animation to render.
    /// </summary>
    /// <returns>The sprite animation.</returns>
    protected abstract QueueableSpriteAnimation? GetCurrentAnimation();

    /// <summary>
    /// Handles when an animation finishes.
    /// </summary>
    protected abstract void HandleAnimationFinished();

    private void FrameRateOverride_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.ResetFrameRate();
    }

    private void ResetAnimation() {
        this.GetCurrentAnimation()?.Reset();
    }

    private void ResetFrameRate() {
        if (this.FrameRateOverride is { IsEnabled: true, Value: > 0 }) {
            this.MillisecondsPerFrame = 1000 / this.FrameRateOverride.Value;
        }
        else {
            this.MillisecondsPerFrame = 0;
        }
    }

    private bool TryGetCurrentAnimation([NotNullWhen(true)] out QueueableSpriteAnimation? animation) {
        animation = this.IsPlaying ? this.GetCurrentAnimation() : null;
        return animation != null;
    }
}