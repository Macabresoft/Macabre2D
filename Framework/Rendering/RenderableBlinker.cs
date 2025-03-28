namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// As the child of a <see cref="IRenderableEntity" />, can blink that entity like a retro video game.
/// </summary>
public class RenderableBlinker : UpdateableEntity {
    private byte _currentNumberOfBlinks;
    private Action? _finishedCallback;
    private byte _totalNumberOfBlinks;

    /// <summary>
    /// Gets the timer for the amount of time that the <see cref="IRenderableEntity" /> will disappear while blinking.
    /// </summary>
    [DataMember]
    public GameTimer DisappearTimer { get; } = new(0.5f);

    /// <summary>
    /// Gets the timer for the amount of time that the <see cref="IRenderableEntity" /> will show between blinks.
    /// </summary>
    [DataMember]
    public GameTimer ShowTimer { get; } = new(0.5f);

    /// <summary>
    /// Begins blinking the parent <see cref="IRenderableEntity" /> if it exists.
    /// </summary>
    /// <param name="numberOfBlinks">The number of blinks before finishing.</param>
    /// <param name="finishedCallback">The finished callback.</param>
    public void BeginBlink(byte numberOfBlinks, Action? finishedCallback) {
        if (numberOfBlinks > 0 && this.Parent is IRenderableEntity renderable) {
            this._totalNumberOfBlinks = numberOfBlinks;
            this._currentNumberOfBlinks = 0;
            this._finishedCallback = finishedCallback;
            this.DisappearTimer.Restart();
            renderable.ShouldRender = false;
            this.ShouldUpdate = true;
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        this.ShouldUpdate = false;
        base.Initialize(scene, parent);
    }

    /// <summary>
    /// Stops the blinking immediately.
    /// </summary>
    /// <param name="shouldRender">A value indicating whether this should stop blinking.</param>
    public void Stop(bool shouldRender) {
        this._finishedCallback = null;
        this.OnFinished();

        if (this.Parent is IRenderableEntity renderable) {
            renderable.ShouldRender = shouldRender;
        }
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Parent is IRenderableEntity renderable) {
            if (this._currentNumberOfBlinks >= this._totalNumberOfBlinks) {
                this.OnFinished();
            }
            else if (this.DisappearTimer.State == TimerState.Running) {
                this.DisappearTimer.Increment(frameTime);

                if (this.DisappearTimer.State == TimerState.Finished) {
                    renderable.ShouldRender = true;
                    this.ShowTimer.Restart();
                }
            }
            else if (this.ShowTimer.State == TimerState.Running) {
                this.ShowTimer.Increment(frameTime);

                if (this.ShowTimer.State == TimerState.Finished) {
                    renderable.ShouldRender = false;
                    this.DisappearTimer.Restart();
                    this._currentNumberOfBlinks++;
                }
            }
        }
    }

    private void OnFinished() {
        this.DisappearTimer.Stop();
        this.ShowTimer.Stop();
        this._currentNumberOfBlinks = 0;
        this._totalNumberOfBlinks = 0;
        this.ShouldUpdate = false;
        this._finishedCallback?.Invoke();
        this._finishedCallback = null;
    }
}