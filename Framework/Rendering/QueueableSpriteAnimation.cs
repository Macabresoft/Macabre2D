namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;

/// <summary>
/// A wrapper for <see cref="SpriteAnimation" /> that allows it to be queued.
/// </summary>
public class QueueableSpriteAnimation {
    private int _currentFrameIndex;
    private int _currentStepIndex;

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
        this.Reset();
    }

    /// <summary>
    /// Gets the animation.
    /// </summary>
    /// <value>The animation.</value>
    public SpriteAnimation Animation { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this should loop indefinitely when no other animation
    /// has been queued.
    /// </summary>
    /// <value>
    /// <c>true</c> if this should loop indefinitely when no other animation has been queued;
    /// otherwise, <c>false</c>.
    /// </value>
    public bool ShouldLoopIndefinitely { get; set; }

    /// <summary>
    /// Gets the sprite sheet associated with this animation.
    /// </summary>
    public SpriteSheet? SpriteSheet => this.Animation?.SpriteSheet;

    /// <summary>
    /// Gets or sets the current sprite index.
    /// </summary>
    public byte? CurrentSpriteIndex { get; private set; }

    /// <summary>
    /// Gets or sets the milliseconds that have passed for this animation.
    /// </summary>
    public double MillisecondsPassed { get; set; }

    /// <summary>
    /// Gets the percentage complete for the current animation.
    /// </summary>
    /// <returns>The percentage complete.</returns>
    public float GetPercentageComplete() {
        var result = 0f;

        if (this._currentStepIndex < this.Animation.Steps.Count) {
            var totalFrames = this.Animation.Steps.Sum(x => x.Frames);

            if (totalFrames > 0) {
                var currentFrames = this.Animation.Steps.Take(this._currentStepIndex).Sum(x => x.Frames) + this._currentFrameIndex;
                result = currentFrames / (float)totalFrames;
            }

            return result;
        }

        return result;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public void Reset() {
        this.MillisecondsPassed = 0;
        this._currentFrameIndex = 0;
        this._currentStepIndex = 0;
        this.CurrentSpriteIndex = this.GetCurrentStep()?.SpriteIndex;
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="millisecondsPerFrame">The milliseconds per frame.</param>
    /// <param name="isAnimationOver">A value indicating whether or not the animation is over.</param>
    public virtual void Update(FrameTime frameTime, int millisecondsPerFrame, out bool isAnimationOver) {
        isAnimationOver = false;
        this.MillisecondsPassed += frameTime.MillisecondsPassed;

        if (this.MillisecondsPassed >= millisecondsPerFrame) {
            while (this.MillisecondsPassed >= millisecondsPerFrame) {
                this.MillisecondsPassed -= millisecondsPerFrame;
                this._currentFrameIndex++;
            }

            var currentStep = this.GetCurrentStep();
            if (currentStep == null) {
                isAnimationOver = true;
            }
            else if (this._currentFrameIndex >= currentStep.Frames) {
                this._currentFrameIndex = 0;
                this._currentStepIndex++;

                if (this._currentStepIndex >= this.Animation.Steps.Count) {
                    if (this.ShouldLoopIndefinitely) {
                        this._currentStepIndex = 0;
                        this.CurrentSpriteIndex = this.GetCurrentStep()?.SpriteIndex;
                    }
                    else {
                        this._currentStepIndex -= 1;
                        isAnimationOver = true;
                    }
                }
                else {
                    this.CurrentSpriteIndex = this.GetCurrentStep()?.SpriteIndex;
                }
            }
        }
    }

    private SpriteAnimationStep? GetCurrentStep() {
        return this._currentStepIndex < this.Animation.Steps.Count ? this.Animation.Steps.ElementAt(this._currentStepIndex) : null;
    }
}