namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;

public abstract class BaseSpriteAnimator : BaseSpriteEntity, IUpdateableEntity {
    private byte _frameRate = 30;
    private uint _millisecondsPassed;
    private uint _millisecondsPerFrame;
    private bool _isPlaying;

    /// <inheritdoc />
    public override byte? SpriteIndex => this.CurrentSpriteIndex;

    /// <inheritdoc />
    public int UpdateOrder => 0;

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

    /// <summary>
    /// Gets or sets the current frame index.
    /// </summary>
    protected uint CurrentFrameIndex { get; set; }

    /// <summary>
    /// Gets or sets the current sprite index.
    /// </summary>
    protected byte? CurrentSpriteIndex { get; set; }

    /// <summary>
    /// Gets or sets the current step index.
    /// </summary>
    protected uint CurrentStepIndex { get; set; }

    /// <summary>
    /// Gets the percentage complete for the current animation.
    /// </summary>
    /// <returns>The percentage complete.</returns>
    public float GetPercentageComplete() {
        var result = 0f;

        if (this.GetSpriteAnimation() is { } animation && this.CurrentStepIndex < animation.Steps.Count) {
            var totalFrames = animation.Steps.Sum(x => x.Frames);

            if (totalFrames > 0) {
                var currentFrames = animation.Steps.Take((int)this.CurrentStepIndex).Sum(x => x.Frames) + this.CurrentFrameIndex;
                result = currentFrames / (float)totalFrames;
            }

            return result;
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
    /// Plays this instance.
    /// </summary>
    public void Play() {
        this.IsEnabled = true;
        this.IsPlaying = true;

        if (this.GetSpriteAnimation() is { } animation && animation.Steps.Count > this.CurrentStepIndex) {
            var currentStep = animation.Steps.ElementAt((int)this.CurrentStepIndex);
            this.CurrentSpriteIndex = currentStep.SpriteIndex;
        }

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

    /// <inheritdoc />
    public virtual void Update(FrameTime frameTime, InputState inputState) {
        if (this.IsPlaying && this.GetSpriteAnimation() is { } initialAnimation) {
            if (this.HandleAnimationFinished(initialAnimation) is { } currentAnimation) {
                this._millisecondsPassed += Convert.ToUInt32(frameTime.MillisecondsPassed);

                if (this._millisecondsPassed >= this._millisecondsPerFrame) {
                    while (this._millisecondsPassed >= this._millisecondsPerFrame) {
                        this._millisecondsPassed -= this._millisecondsPerFrame;
                        this.CurrentFrameIndex++;
                    }

                    var currentStep = currentAnimation.Steps.ElementAt((int)this.CurrentStepIndex);
                    if (this.CurrentFrameIndex >= currentStep.Frames) {
                        this.CurrentFrameIndex = 0;
                        this.CurrentStepIndex++;

                        if (this.HandleAnimationFinished(currentAnimation) is { } nextAnimation) {
                            currentStep = nextAnimation.Steps.ElementAt((int)this.CurrentStepIndex);
                            this.CurrentSpriteIndex = currentStep.SpriteIndex;
                        }
                        else {
                            this.ResetAnimation();
                        }
                    }
                }
            }
            else {
                this.ResetAnimation();
            }
        }
    }

    /// <summary>
    /// Gets the sprite animation to render.
    /// </summary>
    /// <returns>The sprite animation.</returns>
    protected abstract SpriteAnimation? GetSpriteAnimation();

    /// <summary>
    /// Handles when an animation finishes.
    /// </summary>
    /// <returns>The animation to continue with.</returns>
    protected abstract SpriteAnimation? HandleAnimationFinished();

    /// <summary>
    /// Resets the animation.
    /// </summary>
    protected void ResetAnimation() {
        this._millisecondsPassed = 0;
        this.CurrentFrameIndex = 0;
        this.CurrentStepIndex = 0;
    }

    /// <summary>
    /// Resets the milliseconds passed.
    /// </summary>
    protected void ResetMillisecondsPassed() {
        this._millisecondsPassed = 0;
    }

    private SpriteAnimation? HandleAnimationFinished(SpriteAnimation spriteAnimation) {
        if (this.CurrentStepIndex >= spriteAnimation.Steps.Count) {
            this.CurrentStepIndex = 0;
            return this.HandleAnimationFinished();
        }

        return spriteAnimation;
    }

    private void ResetFrameRate() {
        this._millisecondsPerFrame = 1000u / this._frameRate;
    }
}