namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// A base class for sprite animators.
/// </summary>
public abstract class BaseSpriteAnimator : BaseSpriteEntity, IUpdateableEntity {
    private byte _frameRate = 30;
    private bool _isPlaying;
    private int _millisecondsPerFrame;
    private int _updateOrder;

    /// <summary>
    /// Gets the current animation.
    /// </summary>
    public SpriteAnimation? CurrentAnimation => this.GetCurrentAnimation()?.Animation;

    /// <summary>
    /// Gets a value indicating whether or not this is looping on the current animation.
    /// </summary>
    public bool IsLooping => this.GetCurrentAnimation() is { ShouldLoopIndefinitely: true };

    /// <inheritdoc />
    public override byte? SpriteIndex => this.GetCurrentAnimation()?.CurrentSpriteIndex;

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
        protected set => this.Set(ref this._isPlaying, value);
    }

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get => this._updateOrder;
        set => this.Set(ref this._updateOrder, value);
    }

    /// <inheritdoc />
    protected override SpriteSheetAsset? SpriteSheet => this.CurrentAnimation?.SpriteSheet;

    /// <summary>
    /// Gets the percentage complete for the current animation.
    /// </summary>
    /// <returns>The percentage complete.</returns>
    public float GetPercentageComplete() {
        var result = 0f;

        if (this.GetCurrentAnimation() is { } animation) {
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

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        if (this.IsPlaying && this.GetCurrentAnimation() is { } animation) {
            animation.Update(frameTime, this._millisecondsPerFrame, out var isAnimationOver);

            if (isAnimationOver) {
                this.HandleAnimationFinished();
            }
        }
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

    private void ResetAnimation() {
        this.GetCurrentAnimation()?.Reset();
    }

    private void ResetFrameRate() {
        this._millisecondsPerFrame = this._frameRate > 0 ? this._millisecondsPerFrame = 1000 / this._frameRate : 0;
    }
}