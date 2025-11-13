namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;

/// <summary>
/// Defines the way a <see cref="QueueableSpriteAnimation" /> loops.
/// </summary>
public enum AnimationLoopKind {
    None,
    NoneReverse,
    Repeating,
    RepeatingReverse,
    PingPong
}

/// <summary>
/// A wrapper for <see cref="SpriteAnimation" /> that allows it to be queued.
/// </summary>
public class QueueableSpriteAnimation {
    private int _currentFrameIndex;
    private int _currentStepIndex;
    private bool _isReversed;
    private AnimationLoopKind _loopKind;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueableSpriteAnimation" /> class.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="loopKind">The kind of loop this animation performs.</param>
    public QueueableSpriteAnimation(SpriteAnimation animation, AnimationLoopKind loopKind) {
        this.Animation = animation ?? throw new ArgumentNullException(nameof(animation));
        this._loopKind = loopKind;
        this.Reset();
    }

    /// <summary>
    /// Gets the animation.
    /// </summary>
    /// <value>The animation.</value>
    public SpriteAnimation Animation { get; private set; }

    /// <summary>
    /// Gets or sets the current sprite index.
    /// </summary>
    public byte? CurrentSpriteIndex { get; private set; }

    /// <summary>
    /// Gets the identifier for this animation.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets a value indicating whether this is looping.
    /// </summary>
    public bool IsLooping => this.LoopKind is AnimationLoopKind.Repeating or AnimationLoopKind.RepeatingReverse or AnimationLoopKind.PingPong;

    /// <summary>
    /// Gets or sets the kind of loop this performs.
    /// </summary>
    public AnimationLoopKind LoopKind {
        get => this._loopKind;
        set {
            if (value != this._loopKind) {
                this._loopKind = value;
                this.Reset();
            }
        }
    }

    /// <summary>
    /// Gets or sets the milliseconds that have passed for this animation.
    /// </summary>
    public double MillisecondsPassed { get; set; }

    /// <summary>
    /// Gets the sprite sheet associated with this animation.
    /// </summary>
    public SpriteSheet? SpriteSheet => this.Animation.SpriteSheet;

    /// <summary>
    /// Gets the percentage complete for the current animation.
    /// </summary>
    /// <returns>The percentage complete.</returns>
    public float GetPercentageComplete() {
        var result = 0f;

        if (this._currentStepIndex < this.Animation.Steps.Count) {
            var totalFrames = this.Animation.TotalNumberOfFrames;

            if (totalFrames > 0) {
                var currentFrames = this.Animation.Steps.Take(this._currentStepIndex).Sum(x => x.Frames) + this._currentFrameIndex;
                result = currentFrames / (float)totalFrames;
            }
        }

        return result;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public void Reset() {
        this.Reset(0, 0, 0d);
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    /// <param name="stepIndex">The step index.</param>
    /// <param name="frameIndex">The frame index of the current step.</param>
    /// <param name="millisecondsPassed">The milliseconds passed in the current frame.</param>
    public void Reset(int stepIndex, int frameIndex, double millisecondsPassed) {
        this.MillisecondsPassed = millisecondsPassed;
        this._currentFrameIndex = frameIndex;
        this._currentStepIndex = stepIndex;
        this.CurrentSpriteIndex = this.GetCurrentStep()?.SpriteIndex;
        this._isReversed = this.LoopKind == AnimationLoopKind.RepeatingReverse;
    }

    /// <summary>
    /// Sets the percentage complete of the current animation to a value between 0 and 1.
    /// </summary>
    /// <param name="amount">The amount.</param>
    public void SetPercentageComplete(float amount) {
        this._currentFrameIndex = 0;
        this._currentStepIndex = 0;

        var totalFrames = this.Animation.TotalNumberOfFrames;
        var desiredFrameNumber = (int)Math.Round(totalFrames * Math.Clamp(amount, 0f, 1f));

        var currentFrame = 0;
        var currentStep = 0;
        foreach (var step in this.Animation.Steps) {
            currentFrame += step.Frames;

            if (currentFrame >= desiredFrameNumber) {
                currentFrame = step.Frames - (currentFrame - desiredFrameNumber);
                break;
            }

            currentStep++;
        }

        this._currentFrameIndex = currentFrame;
        this._currentStepIndex = currentStep;
        this.MillisecondsPassed = 0d;
    }

    /// <summary>
    /// Swaps in one animation for another while maintaining everything about the current frame and transposing it onto the new animation.
    /// </summary>
    /// <remarks>
    /// This will only work if the new animation has the same or more frames than the current animation.
    /// </remarks>
    public void Swap(SpriteAnimation animation) {
        this.Animation = animation;
    }

    /// <summary>
    /// Tries to move to the next frame.
    /// </summary>
    /// <param name="isAnimationOver">A value indicating whether the animation is over.</param>
    public void TryNextFrame(out bool isAnimationOver) {
        isAnimationOver = false;
        this._currentFrameIndex++;
        this.TryNewStep(out isAnimationOver);
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="millisecondsPerFrame">The milliseconds per frame.</param>
    /// <param name="isAnimationOver">A value indicating whether the animation is over.</param>
    public virtual void Update(FrameTime frameTime, int millisecondsPerFrame, out bool isAnimationOver) {
        isAnimationOver = false;
        this.MillisecondsPassed += frameTime.MillisecondsPassed;

        if (this.MillisecondsPassed >= millisecondsPerFrame && millisecondsPerFrame > 0) {
            while (this.MillisecondsPassed >= millisecondsPerFrame) {
                this.MillisecondsPassed -= millisecondsPerFrame;
                this._currentFrameIndex++;
            }

            this.TryNewStep(out isAnimationOver);
        }
    }

    private SpriteAnimationStep? GetCurrentStep() => this._currentStepIndex < this.Animation.Steps.Count ? this.Animation.Steps.ElementAt(this._currentStepIndex) : null;

    private void TryNewStep(out bool isAnimationOver) {
        isAnimationOver = false;
        var currentStep = this.GetCurrentStep();
        if (currentStep == null) {
            isAnimationOver = true;
        }
        else if (this._currentFrameIndex >= currentStep.Frames) {
            this._currentFrameIndex = 0;

            if (!this._isReversed) {
                this._currentStepIndex++;

                if (this._currentStepIndex >= this.Animation.Steps.Count) {
                    if (this.LoopKind == AnimationLoopKind.None) {
                        this._currentStepIndex = this.Animation.Steps.Count - 1;
                        isAnimationOver = true;
                    }
                    else if (this.LoopKind == AnimationLoopKind.Repeating) {
                        this._currentStepIndex = 0;
                    }
                    else if (this.LoopKind == AnimationLoopKind.PingPong) {
                        this._currentStepIndex = this.Animation.Steps.Count - 2;
                        this._isReversed = true;
                    }
                }
            }
            else {
                this._currentStepIndex--;

                if (this._currentStepIndex < 0) {
                    if (this.LoopKind == AnimationLoopKind.NoneReverse) {
                        this._currentStepIndex = 0;
                        isAnimationOver = true;
                    }
                    else if (this.LoopKind is AnimationLoopKind.RepeatingReverse) {
                        this._currentStepIndex = this.Animation.Steps.Count - 1;
                    }
                    else if (this.LoopKind == AnimationLoopKind.PingPong) {
                        this._currentStepIndex = 1;
                        this._isReversed = !this._isReversed;
                    }
                }
            }
        }

        this.CurrentSpriteIndex = this.GetCurrentStep()?.SpriteIndex;
    }
}