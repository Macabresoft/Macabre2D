namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for sprite animators.
/// </summary>
public interface ISpriteAnimator : IAnimatableEntity, ISpriteEntity {
    /// <summary>
    /// Gets the current animation.
    /// </summary>
    SpriteAnimation? CurrentAnimation { get; }

    /// <summary>
    /// Gets a value indicating whether this is looping.
    /// </summary>
    bool IsLooping { get; }

    /// <summary>
    /// Gets a value indicating whether this is playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Gets the animation percentage complete as a value between 0 and 1.
    /// </summary>
    /// <returns>The percentage completed.</returns>
    float GetPercentageComplete();

    /// <summary>
    /// Pauses this instance.
    /// </summary>
    void Pause();

    /// <summary>
    /// Plays this instance.
    /// </summary>
    void Play();

    /// <summary>
    /// Stops this instance.
    /// </summary>
    void Stop();
}

/// <summary>
/// A base class for sprite animators.
/// </summary>
public abstract class BaseSpriteAnimator : BaseSpriteEntity, ISpriteAnimator {
    private bool _isPlaying;

    /// <inheritdoc />
    public event EventHandler? ShouldAnimateChanged;

    /// <inheritdoc />
    public SpriteAnimation? CurrentAnimation => this.GetCurrentAnimation()?.Animation;

    /// <inheritdoc />
    [DataMember(Order = 11, Name = "Frame Rate")]
    public ByteOverride FrameRateOverride { get; } = new();

    /// <inheritdoc />
    public bool IsLooping => this.GetCurrentAnimation() is { ShouldLoopIndefinitely: true };

    /// <inheritdoc />
    public bool IsPlaying {
        get => this._isPlaying;
        protected set {
            if (this.Set(ref this._isPlaying, value) && this.IsEnabled) {
                this.ShouldAnimateChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    public bool ShouldAnimate => this.CheckShouldAnimate();

    /// <inheritdoc />
    public override byte? SpriteIndex => this.GetCurrentAnimation()?.CurrentSpriteIndex;

    /// <summary>
    /// Get the number of milliseconds in a single frame.
    /// </summary>
    protected int MillisecondsPerFrame { get; private set; }

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this.CurrentAnimation?.SpriteSheet;

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

    /// <inheritdoc />
    public void Pause() {
        this.IsPlaying = false;
    }

    /// <inheritdoc />
    public void Play() {
        this.IsPlaying = true;
        this.Reset();
    }

    /// <inheritdoc />
    public virtual void Stop() {
        this.IsPlaying = false;
        this.ResetAnimation();
    }

    /// <summary>
    /// Checks whether this should animate.
    /// </summary>
    /// <remarks>
    /// Some animations may want to run the animation loop even when <see cref="IsPlaying" /> does not exist. This allows more control.
    /// </remarks>
    /// <returns>A value indicating whether this should animate.</returns>
    protected virtual bool CheckShouldAnimate() => this.IsEnabled && this.IsPlaying;

    /// <summary>
    /// Gets the sprite animation to render.
    /// </summary>
    /// <returns>The sprite animation.</returns>
    protected abstract QueueableSpriteAnimation? GetCurrentAnimation();

    /// <summary>
    /// Handles when an animation finishes.
    /// </summary>
    protected abstract void HandleAnimationFinished();

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();
        if (this.IsPlaying) {
            this.ShouldAnimateChanged.SafeInvoke(this);
        }
    }

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