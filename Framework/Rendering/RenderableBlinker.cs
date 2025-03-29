namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Interface for an entity which can control <see cref="IRenderableEntity" /> visibility in a blinking pattern.
/// </summary>
public interface IRenderableBlinker : IUpdateableEntity {

    /// <summary>
    /// Gets the timer for a delay before blinking.
    /// </summary>
    GameTimer DelayTimer { get; }

    /// <summary>
    /// Gets the timer for the amount of time that the <see cref="IRenderableEntity" /> will disappear while blinking.
    /// </summary>
    GameTimer DisappearTimer { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to end immediately while disappeared or to wait for the final disappear timer.
    /// </summary>
    [DataMember]
    public bool EndImmediately { get; set; }

    /// <summary>
    /// Gets the timer for the amount of time that the <see cref="IRenderableEntity" /> will show between blinks.
    /// </summary>
    GameTimer ShowTimer { get; }

    /// <summary>
    /// Begins blinking the parent <see cref="IRenderableEntity" /> if it exists.
    /// </summary>
    /// <param name="numberOfBlinks">The number of blinks before finishing.</param>
    /// <param name="finishedCallback">The finished callback.</param>
    void BeginBlink(byte numberOfBlinks, Action? finishedCallback);

    /// <summary>
    /// Stops the blinking immediately.
    /// </summary>
    /// <param name="shouldRender">A value indicating whether this should stop blinking.</param>
    void Stop(bool shouldRender);
}

/// <summary>
/// As the child of a <see cref="IRenderableEntity" />, can blink that entity like a retro video game.
/// </summary>
public class RenderableBlinker : UpdateableEntity, IRenderableBlinker {
    private byte _currentNumberOfBlinks;
    private Action? _finishedCallback;
    private byte _totalNumberOfBlinks;

    /// <inheritdoc />
    [DataMember]
    public GameTimer DelayTimer { get; } = new(0f);

    /// <inheritdoc />
    [DataMember]
    public GameTimer DisappearTimer { get; } = new(0.5f);

    /// <inheritdoc />
    [DataMember]
    public bool EndImmediately { get; set; }

    /// <inheritdoc />
    [DataMember]
    public GameTimer ShowTimer { get; } = new(0.5f);

    /// <inheritdoc />
    public void BeginBlink(byte numberOfBlinks, Action? finishedCallback) {
        if (numberOfBlinks > 0 && this.Parent is IRenderableEntity renderable) {
            this._totalNumberOfBlinks = numberOfBlinks;
            this._currentNumberOfBlinks = 0;
            this._finishedCallback = finishedCallback;

            this.DisappearTimer.Restart();

            if (this.DelayTimer.TimeLimit > 0f) {
                this.DelayTimer.Restart();
                renderable.ShouldRender = true;
            }
            else {
                renderable.ShouldRender = false;
            }

            this.ShouldUpdate = true;
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        this.ShouldUpdate = false;
        base.Initialize(scene, parent);
    }

    /// <inheritdoc />
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
            if (this.DelayTimer.State == TimerState.Running) {
                this.DelayTimer.Increment(frameTime);
            }
            else if (this.DisappearTimer.State == TimerState.Running) {
                this.DisappearTimer.Increment(frameTime);

                if (this.DisappearTimer.State == TimerState.Finished) {
                    renderable.ShouldRender = true;
                    this.ShowTimer.Restart();

                    if (!this.EndImmediately && this._currentNumberOfBlinks >= this._totalNumberOfBlinks) {
                        this.OnFinished();
                    }
                }
            }
            else if (this.ShowTimer.State == TimerState.Running) {
                this.ShowTimer.Increment(frameTime);

                if (this.ShowTimer.State == TimerState.Finished) {
                    renderable.ShouldRender = false;
                    this.DisappearTimer.Restart();
                    this._currentNumberOfBlinks++;

                    if (this.EndImmediately && this._currentNumberOfBlinks >= this._totalNumberOfBlinks) {
                        this.OnFinished();
                    }
                }
            }
        }
    }

    private void OnFinished() {
        this.DelayTimer.Stop();
        this.DisappearTimer.Stop();
        this.ShowTimer.Stop();
        this._currentNumberOfBlinks = 0;
        this._totalNumberOfBlinks = 0;
        this.ShouldUpdate = false;
        this._finishedCallback?.Invoke();
        this._finishedCallback = null;
    }
}